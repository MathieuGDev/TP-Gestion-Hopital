# Etape 1

Comment garantissez-vous l'unicite du numero de dossier ?

L'unicite du numero de dossier est garantie au niveau de la base de donnees via le DbContext. Dans la methode OnModelCreating de HospitalDbContext, on configure un index unique sur la colonne FolderNumber :

```csharp
modelBuilder.Entity<Models.Patient>()
    .HasIndex(p => p.FolderNumber)
    .IsUnique();
```

Cela genere une contrainte UNIQUE INDEX en base lors de la migration, ce qui empeche l'insertion de deux patients avec le meme numero de dossier.

---

Quelle strategie utilisez-vous pour les cles primaires ?

Les cles primaires utilisent la strategie par convention d'EF Core : chaque entite possede une propriete Id de type int annotee avec [Key]. EF Core configure automatiquement cette colonne en INTEGER PRIMARY KEY AUTOINCREMENT dans SQLite, ce qui signifie que la valeur est generee automatiquement par la base de donnees a chaque insertion.

```csharp
[Key]
public int Id { get; set; }
```

---

Comment validez-vous la date de naissance ?

La validation est assuree par un attribut personnalise [DateInPast], defini dans DateInPastAttribute qui herite de ValidationAttribute. Il verifie que la date est strictement anterieure a aujourd'hui (date.Date < DateTime.Today). Si ce n'est pas le cas, une erreur de validation est retournee. Il est applique sur la propriete DateOfBirth du modele Patient :

```csharp
[DateInPast]
public DateTime DateOfBirth { get; set; }
```

---

# Etape 2

Quelle relation entre Doctor et Department ?

La relation est Many-to-One (plusieurs medecins pour un departement) : un Doctor appartient a un seul Department, et un Department peut avoir plusieurs Doctors. Elle est modelisee par une propriete de navigation et une cle etrangere dans Doctor :

```csharp
// Doctor.cs
public int DepartmentId { get; set; }
public Department Department { get; set; } = null!;

// HospitalDbContext.cs
modelBuilder.Entity<Models.Doctor>()
    .HasOne(d => d.Department)
    .WithMany(dep => dep.Doctors)
    .HasForeignKey(d => d.DepartmentId)
    .OnDelete(DeleteBehavior.Restrict);
```

Le Department connait egalement son responsable via une relation One-to-One nullable vers Doctor :

```csharp
// Department.cs
public int? HeadDoctorId { get; set; }
public Doctor? HeadDoctor { get; set; }
```

---

Quel DeleteBehavior ? Justifiez votre choix

Deux comportements sont configures :

1. DeleteBehavior.Restrict sur la relation Doctor -> Department.
   Si on tente de supprimer un departement qui contient encore des medecins, la base de donnees rejette l'operation avec une erreur. C'est le comportement le plus sur : il force a reassigner ou supprimer les medecins manuellement avant de pouvoir supprimer le departement, evitant toute perte de donnees silencieuse.

2. DeleteBehavior.NoAction sur la relation Department -> HeadDoctor.
   Cette relation est circulaire : un Department reference un Doctor, et ce Doctor reference le meme Department. Si on utilisait Restrict ou Cascade des deux cotes, EF Core (et SQLite) genererait une cascade circulaire impossible a resoudre. NoAction indique qu'aucune action automatique n'est prise en base, laissant la responsabilite a l'application de gerer la coherence.

---

# Etape 5

## Comparez Eager Loading vs Lazy Loading pour ces cas d'usage

**Eager Loading** (`Include` / `ThenInclude`) charge toutes les donnees liees en une seule requete SQL avec des JOINs, au moment ou la requete principale est executee.

**Lazy Loading** charge les donnees liees a la demande, uniquement quand on accede a la propriete de navigation pour la premiere fois. Chaque acces genere une nouvelle requete SQL separee.

Pour la fiche patient (Patient + Consultations + Doctor + Department), l'Eager Loading est clairement preferable : on sait a l'avance que la vue a besoin de toutes ces donnees. Avec le Lazy Loading, une boucle sur 10 consultations pour afficher le nom du medecin de chacune genererait 10 requetes SQL supplementaires.

---

## Quand est-il pertinent d'utiliser des projections ?

Une projection consiste a utiliser `Select(...)` pour ne recuperer que les colonnes reellement necessaires, plutot que de charger une entite complete avec tous ses champs et ses navigations.

Elle est pertinente dans trois situations :

1. **Lecture seule sans modification** : quand on n'a pas besoin de tracker l'entite (tableaux de statistiques, listes legeres). C'est le cas de `DepartmentStats()` qui calcule des agregats directement en SQL sans jamais materialiser un objet `Department` complet.

2. **Vues agregees** : quand on a besoin de `Count()`, `Sum()`, `Max()` sur des collections liees. EF Core traduit ces expressions en `GROUP BY` / fonctions d'agregation SQL, bien plus efficaces que charger toutes les lignes en memoire et compter cote C#.

3. **API retournant un sous-ensemble de champs** : eviter d'exposer des champs sensibles ou inutiles dans la reponse JSON (comme dans `GetUpcomingForPatient` qui projette uniquement `Id`, `FirstName`, `LastName`, `Specialty` du medecin).

---

## Quel impact sur les performances si on charge toutes les donnees sans filtre ?

Charger toutes les donnees sans filtre a plusieurs impacts negatifs :

**Cote base de donnees :**
- La requete SQL ne contient pas de clause `WHERE`, ce qui oblige la base a parcourir toutes les lignes de chaque table (full table scan).
- Si des `Include` sont ajoutes sans filtre, les JOINs multiplient le nombre de lignes retournees : 100 patients x 10 consultations chacun = 1 000 lignes rapatriees pour afficher une simple liste de patients.

**Cote application :**
- Toutes les entites sont instanciees et trackees par le change tracker d'EF Core (sauf `AsNoTracking`), ce qui consomme de la memoire et du CPU inutilement.
- Le serialiseur JSON (pour les API) doit parcourir et serialiser des graphes d'objets potentiellement circulaires et volumineux.

**Exemple concret :** dans `DoctorDetail()`, le filtre est applique directement dans le `Include` (Filtered Include) :

```csharp
.Include(d => d.Consultations
    .Where(c => c.AppointmentDate >= DateTime.Today
             && c.Status != ConsultationStatus.Cancelled)
    .OrderBy(c => c.AppointmentDate))
```

Sans ce filtre, on chargerait l'integralite de l'historique des consultations du medecin alors que la vue n'affiche que les consultations a venir. Le filtre est traduit en `WHERE` SQL, jamais execute cote C#.
