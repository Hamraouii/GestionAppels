export interface TypeDemande {
  id: string;
  intituleDemande: string;
  descriptionDemande: string;
  sousTypes?: SousTypeDemande[];
}

export interface SousTypeDemande {
  id: string;
  intitule: string;
  description: string;
  typeDemandeId: string;
}

export interface CreateSousTypeDemandePayload {
  intitule: string;
  description: string;
  typeDemandeId: string;
}
