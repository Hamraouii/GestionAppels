import { StatutDemande } from './statut-demande.enum';

export interface CreateFiche {
  telephone1: string;
  telephone2?: string | null;
  telephone3?: string | null;
  affiliation: string;
  details: string;
  sousTypeDemandeId: number;
  userId: string; // Assuming this is the ID of the logged-in user, to be set by the frontend
  statut: StatutDemande;
}
