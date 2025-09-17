export class Planet {
    id: number;
    name: string | null;
    type: string | null;
    dimension: string | null;

    constructor()
    constructor(
        id: number,
        name: string | null,
        type: string | null,
        dimension: string | null
    );
    constructor(
        id?: number,
        name?: string | null,
        type?: string | null,
        dimension?: string | null
    ) {
        this.id = id ?? 0;
        this.name = name ?? null;
        this.type = type ?? null;
        this.dimension = dimension ?? null;
    }
}