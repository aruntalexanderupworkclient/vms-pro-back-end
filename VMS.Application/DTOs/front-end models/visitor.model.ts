export interface Visitor {
    id: number;
    name: string;
    idType: string;
    idNumber: string;
    phone: string;
    purpose: string;
    hostId: number;
    hostName: string;
    photo: string | null;
    checkIn: string;
    checkOut: string | null;
    status: string;
    tokenId: number | null;
    expectedDuration: string;
    orgType: string;
}
