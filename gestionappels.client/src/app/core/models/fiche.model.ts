import { StatutDemande } from './statut-demande.enum';

export interface Fiche {
  id: string; // Guid is string in TypeScript
  telephone1: string;
  telephone2?: string | null;
  telephone3?: string | null;
  affiliation: string;
  adherentNom: string;
  details: string;
  sousTypeDemandeId: string;
  sousTypeDemandeIntitule: string;
  userId: string; // Guid is string
  userNom: string;
  statut: StatutDemande;
  createdAt: string; // DateTime is string (ISO 8601)
  updatedAt?: string | null; // DateTime? is string or null
}
