export interface Permission {
    id: number;
    key: string;
    module: string;
    action: string;
    roles: string[];
}
