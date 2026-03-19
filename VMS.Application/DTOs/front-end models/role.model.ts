export interface Role {
    id: number;
    name: string;
    description: string;
    usersCount: number;
    permissions: { [module: string]: { view: boolean; create: boolean; edit: boolean; delete: boolean } };
}
