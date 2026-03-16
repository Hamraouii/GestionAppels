export interface SousTypeDemandeDto {
  id: string;
  intitule: string;
  description: string;
  typeDemandeId?: string; // This is not present in the nested objects from the API
}

export interface TypeDemandeDto {
  id: string;
  intituleDemande: string;
  descriptionDemande: string;
  sousTypeDemandes: SousTypeDemandeDto[];
}
