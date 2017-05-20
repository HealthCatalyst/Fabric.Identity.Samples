export class Permission {
    public Id: string;
    public CreateDateTimeUtc: Date;
    public ModifiedDateTimeUtc: Date;
    public CreatedBy: string;
    public ModifiedBy: string;

    constructor(       
        public Name: string,  
        public Grain: string, 
        public SecurableItem: string,               
    ){}
}