export interface Photo {
    id: number;
    url: string;
    description: string;
    publiccId: string;
    dateAdded: Date;
    isMain: boolean;
    isApproved: boolean;
}
