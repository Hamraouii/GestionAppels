import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule, ParamMap } from '@angular/router';
import { FicheService } from '../../../../core/services/fiche.service';
import { StatutDemande } from '../../../../core/models/statut-demande.enum';
import { Observable, of, Subscription, fromEvent, forkJoin } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap, catchError, map } from 'rxjs/operators';
import { AdherentService } from '../../../../core/services/adherent.service';
import { AdherentSearchResult } from '../../../../core/models/adherent-search-result.model';
import { Fiche } from '../../../../core/models/fiche.model';
import { TypeDemandeDto, SousTypeDemandeDto } from '../../../../core/models/demande.model'; // Added
import { QuillModule } from 'ngx-quill';

@Component({
  selector: 'app-fiche-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, QuillModule],
  templateUrl: './fiche-form.component.html',
  styleUrls: ['./fiche-form.component.scss']
})
export class FicheFormComponent implements OnInit, OnDestroy, AfterViewInit {
  public console = console;
  
  private subscriptions: Subscription[] = [];
  ficheForm: FormGroup;
  isEditMode = false;
  isViewMode = false;
  ficheId: string | null = null;
  isLoading = false;
  error: string | null = null;
  statutDemandeOptions: { key: string, value: number }[] = [];

  // For Demande Dropdowns
  typeDemandes$!: Observable<TypeDemandeDto[]>;
  sousTypeDemandes$!: Observable<SousTypeDemandeDto[]>;

  // For Adherent Autocomplete
  adherentSearchResults$!: Observable<AdherentSearchResult[]>;
  showAdherentResults = false;
  
  @ViewChild('adherentSearchInput') adherentSearchInput?: ElementRef;

  constructor(
    private fb: FormBuilder,
    private ficheService: FicheService,
    private adherentService: AdherentService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    console.log('FicheFormComponent: Constructor called');
    console.log('FicheFormComponent: Creating form...');
    
    this.ficheForm = this.fb.group({
      typeDemandeId: [null, Validators.required], // Added
      adherentSearch: [''],
      affiliation: ['', Validators.required],
      telephone1: ['', Validators.required],
      telephone2: [''],
      telephone3: [''],
      details: ['', Validators.required],
      sousTypeDemandeId: [null, Validators.required],
      // userId is handled by the backend based on the authenticated user
      statut: [StatutDemande.Receptionne, Validators.required]
    });
    
    this.setupAdherentSearch();
    this.setupDemandeDropdowns();
    this.populateStatutDemandeOptions(); // Added call
  }

  private setupAdherentSearch(): void {
    const adherentSearchControl = this.ficheForm.get('adherentSearch');
    
    if (!adherentSearchControl) {
      console.error('FicheFormComponent: adherentSearch control is null!');
      return;
    }
    
        this.adherentSearchResults$ = adherentSearchControl.valueChanges.pipe(
      tap((value: string) => {
      }),
      debounceTime(300),
      distinctUntilChanged(),
      tap((value: string) => {
        // Make sure to set showAdherentResults to true when there's a term
        this.showAdherentResults = !!value && value.trim() !== '';
      }),
      switchMap((term: string) => {
        if (!term || term.trim() === '') {
          return of([]); 
        }
        return this.adherentService.searchAdherents(term).pipe(
          catchError(error => {
            console.error('PIPELINE ERROR: Error in search:', error);
            return of([]);
          })
        );
      }),
      tap(
        (results: AdherentSearchResult[]) => {
          if (results && results.length > 0) {
            this.showAdherentResults = true;
          }
        },
        error => console.error('PIPELINE ERROR in adherentSearchResults$:', error)
      )
    );
  }

  private setupDemandeDropdowns(): void {
    this.typeDemandes$ = this.ficheService.getAllTypeDemandes().pipe(
      catchError(err => {
        console.error('Error loading TypeDemandes:', err);
        this.error = 'Erreur lors du chargement des types de demande.';
        return of([]);
      })
    );

    const typeDemandeIdControl = this.ficheForm.get('typeDemandeId');
    if (typeDemandeIdControl) {
      this.sousTypeDemandes$ = typeDemandeIdControl.valueChanges.pipe(
        switchMap((typeId: string) => {
          if (typeId) {
            return this.ficheService.getSousTypeDemandesByTypeId(typeId).pipe(
              catchError(err => {
                console.error('Error loading SousTypeDemandes:', err);
                this.error = 'Erreur lors du chargement des sous-types de demande.';
                this.ficheForm.get('sousTypeDemandeId')?.setValue(null); // Reset if error
                return of([]);
              })
            );
          } else {
            this.ficheForm.get('sousTypeDemandeId')?.setValue(null); // Reset if no type selected
            return of([]);
          }
        }),
        tap(() => {
          // When typeDemandeId changes, reset sousTypeDemandeId unless we are patching the form
          if (!this.isEditMode || (this.isEditMode && typeDemandeIdControl.dirty)) {
             this.ficheForm.get('sousTypeDemandeId')?.setValue(null);
          }
        })
      );
    }
  }

  private populateStatutDemandeOptions(): void {
    this.statutDemandeOptions = Object.keys(StatutDemande)
      .filter(key => isNaN(Number(key))) // Get string keys: Repondu, Redirige, etc.
      .map(key => ({
        key: key,
        value: StatutDemande[key as keyof typeof StatutDemande] as number
      }));
  }

  ngOnInit(): void {
    this.isLoading = true;
    const ficheId$ = this.route.paramMap.pipe(map((params: ParamMap) => params.get('id')));

    const data$ = ficheId$.pipe(
      switchMap((id: string | null) => {
        this.ficheId = id;
        this.isViewMode = this.route.snapshot.data['viewMode'] === true;
        this.isEditMode = !!this.ficheId && !this.isViewMode;

        if (this.isEditMode || this.isViewMode) {
          // In edit/view mode, fetch the fiche and all typeDemandes in parallel
          return forkJoin({
            fiche: this.ficheService.getFicheById(this.ficheId!),
            typeDemandes: this.typeDemandes$
          });
        }
        // In create mode, no need to fetch a fiche
        return of({ fiche: null, typeDemandes: [] as TypeDemandeDto[] });
      })
    );

    const subscription = data$.subscribe({
      next: ({ fiche, typeDemandes }: { fiche: Fiche | null, typeDemandes: TypeDemandeDto[] }) => {
        if ((this.isEditMode || this.isViewMode) && fiche) {
          if (typeDemandes && typeDemandes.length > 0) {
            const parentType = typeDemandes.find((type: TypeDemandeDto) =>
              type.sousTypeDemandes.some((sousType: SousTypeDemandeDto) => sousType.id === fiche.sousTypeDemandeId)
            );

            this.ficheForm.patchValue({
              ...fiche,
              typeDemandeId: parentType ? parentType.id : null,
            });

            if (fiche.adherentNom && fiche.affiliation) {
              this.ficheForm.get('adherentSearch')?.setValue(`${fiche.adherentNom} (${fiche.affiliation})`);
            } else {
              this.ficheForm.get('adherentSearch')?.setValue(fiche.affiliation);
            }

          } else {
            // If for some reason typeDemandes isn't loaded, still patch the fiche data
            this.ficheForm.patchValue(fiche);
          }

          if (this.isViewMode) {
            this.ficheForm.disable();
          }
        }
        this.isLoading = false;
      },
      error: (err: any) => {
        this.error = "Erreur lors du chargement des données de la fiche.";
        console.error(err);
        this.isLoading = false;
      }
    });

    this.subscriptions.push(subscription);
  }

  ngAfterViewInit(): void {
    
    setTimeout(() => {
      // Manual setup of event listener as a backup approach
      const searchInput = document.getElementById('adherent-search') as HTMLInputElement;
      if (searchInput) {
        const subscription = fromEvent(searchInput, 'input')
          .pipe(
            debounceTime(300),
            distinctUntilChanged(),
          )
          .subscribe((event: Event) => {
            const value = (event.target as HTMLInputElement).value;
            
            // Manually perform search
            if (value && value.trim() !== '') {
              this.adherentService.searchAdherents(value)
                .subscribe({
                  next: results => console.log('Manual search results:', results),
                  error: error => console.error('Manual search error:', error)
                });
            }
          });
        
        this.subscriptions.push(subscription);
      } else {
        console.error('FicheFormComponent: Could not find adherent-search input element');
      }
    }, 500);
  }

  onSubmit(): void {
    if (this.ficheForm.invalid) {
      this.ficheForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.error = null;
    const formValue = this.ficheForm.value;

    const observer = {
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/fiches']);
      },
      error: (err: any) => {
        this.isLoading = false;
        this.error = "Une erreur est survenue lors de la sauvegarde.";
        console.error(err);
      }
    };

    if (this.isEditMode) {
      this.ficheService.updateFiche(this.ficheId!, formValue).subscribe(observer);
    } else {
      this.ficheService.createFiche(formValue).subscribe(observer);
    }
  }

  isInvalid(controlName: string): boolean {
    const control = this.ficheForm.get(controlName);
    return !!control && control.invalid && (control.dirty || control.touched);
  }

  selectAdherent(adherent: AdherentSearchResult): void {
    this.ficheForm.get('affiliation')?.setValue(adherent.affiliation);
    this.ficheForm.get('adherentSearch')?.setValue(`${adherent.nom} ${adherent.prenom} (${adherent.affiliation})`, { emitEvent: false });
    this.showAdherentResults = false;
  }
  
  handleInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    if (target) {
      if (target.value && target.value.trim() !== '') {
        this.showAdherentResults = true;
      }
    }
  }

  cancel(): void {
    this.router.navigate(['/fiches']);
  }

  editCurrentFiche(): void {
    if (this.ficheId) {
      this.router.navigate(['/fiches', this.ficheId, 'edit']);
    }
  }
  
  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
