export class Group {
    
    public Id: string

    constructor(       
        public GroupName: string,
        public GroupSource: string
    ){
        this.Id = GroupName;
    }
}