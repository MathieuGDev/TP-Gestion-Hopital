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
