# Analyse du modele

## Quels sont les avantages et inconvenients de votre modele ?

### Avantages

**Owned Entity pour Address**
L'adresse est un *value object* : elle n'a pas d'identite propre et n'a de sens que rattachee a son proprietaire. En utilisant `OwnsOne`, les colonnes sont stockees dans la meme table que `Patient` ou `Department` (ex. `Patient_Street`, `Patient_City`). Cela evite une jointure supplementaire a chaque lecture, tout en gardant une encapsulation propre cote C#.

**TPH pour le personnel (MedicalStaff)**
La strategie Table Per Hierarchy stocke tous les employes dans une seule table avec une colonne discriminante (`StaffType`). Les requetes transversales (lister tout le personnel, calculer la masse salariale totale) se font sans aucun JOIN. En contrepartie, les colonnes specifiques a chaque sous-type (`Specialty` pour `MedicalDoctor`, `Ward` pour `Nurse`) sont nullables pour toutes les lignes : le schema devient "sparse" et on perd la contrainte NOT NULL au niveau base.

**Hierarchie auto-referencee pour Department**
`ParentDepartmentId` represente des arborescences arbitraires (Cardiologie → Cardiologie Pediatrique) sans table de jonction. Simple a interroger pour un niveau, mais les requetes recursivement profondes (arbre complet) necessitent plusieurs requetes EF Core ou une CTE recursive en SQL brut.

**Relation Doctor ↔ Department avec HeadDoctor**
Le `HeadDoctorId` nullable sur `Department` modelise le chef de service sans entite intermediaire. Le `DeleteBehavior.NoAction` sur cette relation circulaire evite les erreurs de cascade en base, mais delocalise la coherence vers l'application : il faut mettre `HeadDoctorId = null` avant de supprimer un medecin chef.

### Inconvenients

- **Pas de concurrence optimiste** : aucun champ `RowVersion`. Deux utilisateurs modifiant le meme patient simultanement ne detecteront pas le conflit ; le dernier enregistrement ecrase silencieusement l'autre.
- **Pas d'audit trail** : aucune colonne `CreatedAt` / `UpdatedAt`, ni table d'historique. Impossible de savoir qui a modifie une consultation ou quand.
- **SQLite en production** : SQLite serialise les ecritures et ne supporte pas les FK `Restrict` en mode WAL classique. Il n'est pas adapte a la concurrence elevee.
- **Validation uniquement en C#** : les contraintes metier (ex. pas deux consultations chevauchantes pour un medecin) ne sont pas exprimees en base, seulement dans le code applicatif.

---

## Quelles optimisations feriez-vous si l'hopital avait 100 000 patients ?

### Pagination

Mettre en place un système de pagination (opérationnel dans les pages Patients et Médecins)

### Separation lecture / ecriture

Pour les lectures intensives utiliser systematiquement `AsNoTracking()` (deja en place) et des **projections SQL directes** (`Select` vers des DTO plats) plutot que de materialiser des graphes d'entites complets. Cela reduit la consommation memoire du change tracker et le cout de serialisation JSON.

---

## Comment implementeriez-vous un systeme de rendez-vous en ligne ?

### Flux de reservation

1. `GET /api/timeslot?doctorId={id}&date={date}` — liste les creneaux disponibles
2. `POST /api/appointment-request` — le patient reserve un creneau (transition atomique `IsAvailable = false` dans une transaction)
3. Notification asynchrone au medecin (email ou SignalR pour temps reel)
4. `PUT /api/appointment-request/{id}/confirm` ou `/reject` — validation par le secretariat
5. Si confirme : creation d'une entite `Consultation` liee a la `AppointmentRequest`

### Contraintes metier

- Un medecin ne peut pas avoir deux creneaux chevauchants : index unique sur `(DoctorId, Start)` en base.
- Les creneaux passes ne sont plus reservables : validation dans le controller avant tout acces base.

---

## Quel impact sur le modele si on ajoutait la facturation ?

### Nouvelle entite Invoice

La facturation introduit un agregat independant avec son propre cycle de vie (brouillon → emise → payee → annulee) :

```csharp
public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = null!;
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;
    public DateTime IssuedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public InvoiceStatus Status { get; set; }
    public ICollection<InvoiceLine> Lines { get; set; } = new List<InvoiceLine>();
    public decimal TotalAmount => Lines.Sum(l => l.Amount);
}

public class InvoiceLine
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int? ConsultationId { get; set; }
    public Consultation? Consultation { get; set; }
    public string Description { get; set; } = null!;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Amount => UnitPrice * Quantity;
}

public enum InvoiceStatus { Draft, Issued, Paid, Cancelled }
```

### Impacts sur les modeles existants

- **Consultation** : ajouter `bool IsBilled` ou une FK nullable `InvoiceLineId` pour eviter la double facturation.
- **Doctor** : si les honoraires varient par medecin ou specialite, ajouter `decimal ConsultationFee`.
- **Patient** : aucune modification structurelle, mais la vue patient devra afficher le solde ou l'historique de factures.

### Contraintes d'integrite en base

```csharp
modelBuilder.Entity<Invoice>()
    .HasIndex(i => i.InvoiceNumber).IsUnique();

modelBuilder.Entity<Invoice>()
    .Property(i => i.Status)
    .HasConversion<string>(); 

modelBuilder.Entity<InvoiceLine>()
    .Property(l => l.UnitPrice)
    .HasPrecision(10, 2);
```

