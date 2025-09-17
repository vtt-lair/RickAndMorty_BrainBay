import { Planet } from './planet';

export class Character {
    id: number;
    name: string | null;
    species: string | null;
    type: string | null;
    gender: string | null;
    originId: number | null;
    origin: Planet | null;
    locationId: number | null;
    location: Planet | null;
    image: string | null;

    constructor()
    constructor(
        id: number,
        name: string | null,
        species: string | null,
        type: string | null,
        gender: string | null,
        originId: number | null,
        origin: Planet | null,
        locationId: number | null,
        location: Planet | null,
        image: string | null,
    );
    constructor(
        id?: number,
        name?: string | null,
        species?: string | null,
        type?: string | null,
        gender?: string | null,
        originId?: number | null,
        origin?: Planet | null,
        locationId?: number | null,
        location?: Planet | null,
        image?: string | null,
    ) {
        this.id = id ?? 0;
        this.name = name ?? null;
        this.species = species ?? null;
        this.type = type ?? null;
        this.gender = gender ?? null;
        this.originId = originId ?? null;
        this.origin = origin ?? null;
        this.locationId = locationId ?? null;
        this.location = location ?? null;
        this.image = image ?? null;
    }
}