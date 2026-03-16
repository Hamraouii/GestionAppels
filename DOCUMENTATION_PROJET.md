# Documentation Projet - Gestion d'Appels

## Vue d'ensemble du projet

Le projet **Gestion d'Appels** est une application web full-stack développée pour gérer les demandes et fiches d'adhérents dans un système de gestion d'assurance ou de mutuelle. L'application est composée de deux parties principales :

- **Backend** : API REST développée en ASP.NET Core 9.0 (C#)
- **Frontend** : Application web développée en Angular 19

## Architecture Technique

### Backend (GestionAppels.Server)

#### Technologies utilisées
- **Framework** : ASP.NET Core 9.0
- **Base de données** : PostgreSQL avec Entity Framework Core
- **Authentification** : JWT (JSON Web Tokens)
- **ORM** : Entity Framework Core 9.0.6 + Dapper pour certaines requêtes
- **API Documentation** : Swagger/OpenAPI
- **Containerisation** : Docker

#### Structure du projet Backend

```
GestionAppels.Server/
├── Controllers/           # Contrôleurs API REST
│   ├── AccountsController.cs    # Authentification
│   ├── AdherentsController.cs   # Gestion des adhérents
│   ├── DemandesController.cs    # Gestion des demandes
│   ├── FichesController.cs      # Gestion des fiches
│   └── SyncController.cs        # Synchronisation des données
├── Models/               # Modèles de données (Entités)
│   ├── BaseEntity.cs           # Entité de base avec audit
│   ├── Adherent.cs             # Adhérent
│   ├── Fiche.cs                # Fiche d'appel
│   ├── User.cs                 # Utilisateur système
│   ├── SousTypeDemande.cs      # Sous-type de demande
│   ├── TypeDemande.cs          # Type de demande
│   └── ...
├── Dtos/                 # Data Transfer Objects (Records)
├── Services/             # Services métier
│   ├── AccountService.cs       # Service d'authentification
│   ├── AdherentService.cs      # Service adhérents
│   ├── FicheService.cs         # Service fiches
│   ├── DemandeService.cs       # Service demandes
│   └── AdherentSyncService.cs  # Service de synchronisation
├── Data/                 # Contexte de base de données
├── Migrations/           # Migrations Entity Framework
└── Middleware/           # Middleware personnalisés
```

#### Fonctionnalités Backend

1. **Authentification et autorisation**
   - Système d'authentification JWT
   - Gestion des rôles utilisateurs
   - Protection des endpoints par authentification

2. **Gestion des adhérents**
   - CRUD complet sur les adhérents
   - Synchronisation avec base de données externe Oracle
   - Recherche et filtrage

3. **Gestion des fiches d'appels**
   - Création, modification, suppression de fiches
   - Historique des services associés
   - Statuts de demandes
   - Assignation aux utilisateurs

4. **Gestion des demandes**
   - Types et sous-types de demandes
   - Workflow de traitement
   - Suivi des statuts

5. **API de synchronisation**
   - Synchronisation des données adhérents depuis Oracle
   - Gestion des états de synchronisation

#### Configuration de la base de données

Le système utilise PostgreSQL comme base de données principale avec Entity Framework Core. Les entités héritent toutes de `BaseEntity` qui fournit les champs d'audit (`CreatedAt`, `UpdatedAt`, etc.).

### Frontend (gestionappels.client)

#### Technologies utilisées
- **Framework** : Angular 19
- **UI Components** : Angular Material
- **Éditeur de texte** : ngx-quill (Quill.js)
- **Styling** : SCSS
- **Tests** : Jasmine + Karma

#### Structure du projet Frontend

```
gestionappels.client/src/app/
├── core/                 # Services et composants centraux
│   ├── auth/            # Authentification (guards, interceptors)
│   ├── services/        # Services HTTP
│   ├── models/          # Interfaces TypeScript
│   └── layout/          # Composant de mise en page
├── features/            # Modules fonctionnels
│   ├── demandes/        # Module gestion des demandes
│   └── fiches/          # Module gestion des fiches
├── auth/                # Module d'authentification
├── home/                # Page d'accueil
├── shared/              # Composants partagés
└── app-routing.module.ts # Configuration du routage
```

#### Fonctionnalités Frontend

1. **Authentification**
   - Page de connexion
   - Gestion des tokens JWT
   - Guards de route pour la protection

2. **Interface utilisateur**
   - Interface en français (selon les préférences utilisateur)
   - Layout responsive avec sidebar
   - Navigation contextuelle selon les droits

3. **Gestion des fiches**
   - Liste et création de fiches
   - Éditeur riche pour les détails
   - Recherche et filtrage

4. **Gestion des demandes**
   - Types et sous-types de demandes
   - Workflow de traitement
   - Historique des modifications

## Base de données

### Modèle de données principal

#### Entité Adherent
```csharp
public class Adherent : BaseEntity
{
    public string? Nom { get; set; }
    public string? Prenom { get; set; }
    public string? Ville { get; set; }
    public char? Sexe { get; set; }
    public string? Adresse { get; set; }
    public string? Immatriculation { get; set; }
    public string? Cin { get; set; }
    public DateOnly? DateNaissance { get; set; }
    public required string Affiliation { get; set; }
    public int? StatutAdherent { get; set; }
    public virtual ICollection<Fiche> Fiches { get; set; }
}
```

#### Entité Fiche
```csharp
public class Fiche : BaseEntity
{
    public string? Telephone1 { get; set; }
    public string? Telephone2 { get; set; }
    public string? Telephone3 { get; set; }
    public string Affiliation { get; set; } = null!;
    public virtual Adherent Adherent { get; set; } = null!;
    public string? Details { get; set; }
    public Guid SousTypeDemandeId { get; set; }
    public virtual SousTypeDemande SousTypeDemande { get; set; } = null!;
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
    public StatutDemande Statut { get; set; }
    public virtual ICollection<FicheServiceHistory> ServiceHistory { get; set; }
}
```

### Relations principales
- Un **Adherent** peut avoir plusieurs **Fiches**
- Une **Fiche** appartient à un **Adherent** et un **User**
- Une **Fiche** a un **SousTypeDemande**
- Un **SousTypeDemande** appartient à un **TypeDemande**

## Déploiement et DevOps

### Configuration Docker
Le projet utilise Docker pour la containerisation avec un Dockerfile multi-stage pour optimiser la taille de l'image.

### Pipeline CI/CD (GitLab)
Le fichier `.gitlab-ci.yml` définit un pipeline avec les étapes suivantes :
1. **Build** : Construction de l'image Docker
2. **Test** : Exécution des tests Angular et .NET
3. **Package** : Empaquetage de l'application
4. **Deploy** : Déploiement automatique

### Variables d'environnement importantes
- `ConnectionStrings:DefaultConnection` : Chaîne de connexion PostgreSQL
- `Jwt:Key` : Clé secrète pour les tokens JWT
- `Jwt:Issuer` : Émetteur des tokens
- `Jwt:Audience` : Audience des tokens

## Synchronisation des données

Le système inclut un service de synchronisation (`AdherentSyncService`) qui permet de synchroniser les données des adhérents depuis une base Oracle externe. La requête SQL de synchronisation est documentée dans le fichier `MesNotes`.

## Bonnes pratiques implémentées

1. **Architecture en couches** : Séparation claire entre contrôleurs, services et modèles
2. **DTOs** : Utilisation de records pour les Data Transfer Objects
3. **Entity inheritance** : Toutes les entités héritent de `BaseEntity` pour l'audit
4. **Authentification sécurisée** : JWT avec validation stricte
5. **Interface multilingue** : Application entièrement en français
6. **Tests** : Framework de tests configuré pour Angular

## Configuration de développement

### Backend
1. Configurer la chaîne de connexion PostgreSQL dans `appsettings.Development.json`
2. Configurer les paramètres JWT
3. Exécuter les migrations Entity Framework : `dotnet ef database update`
4. Lancer l'application : `dotnet run`

### Frontend
1. Installer les dépendances : `npm install`
2. Lancer en mode développement : `npm start`
3. L'application sera accessible sur `https://localhost:22094`

## Points d'attention pour la maintenance

1. **Sécurité** : Les clés JWT doivent être régulièrement rotées
2. **Performance** : Surveiller les requêtes de synchronisation Oracle
3. **Sauvegarde** : Sauvegardes régulières de la base PostgreSQL
4. **Monitoring** : Logs applicatifs configurés pour le suivi des erreurs
5. **Mise à jour** : Framework Angular 19 et .NET 9.0 relativement récents

## Contacts et ressources

- **Environnement de développement** : Windows avec PowerShell
- **IDE recommandé** : Visual Studio ou Visual Studio Code
- **Base de données** : PostgreSQL pour les données principales, Oracle pour la synchronisation
- **Documentation API** : Swagger UI disponible en mode développement

---

*Cette documentation a été générée lors du départ de l'équipe de développement original. Pour toute question technique, se référer au code source et aux commentaires intégrés.*
