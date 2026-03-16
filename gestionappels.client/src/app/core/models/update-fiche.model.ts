import { StatutDemande } from './statut-demande.enum';

export interface UpdateFiche {
  telephone1?: string;
  telephone2?: string | null;
  telephone3?: string | null;
  details?: string;
  sousTypeDemandeId?: number;
  // UserId is typically not updatable directly, or set by backend based on who is making the update
  statut?: StatutDemande;
}
