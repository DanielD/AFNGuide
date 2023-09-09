export class SearchResultItemModel {
  constructor(
    public Id: number,
    public Title: string,
    public ChannelId: number,
    public IsPremiere: boolean,
    public AirDateLocal: Date,
    public EpisodeTitle?: string,
    public Description?: string,
    public Duration?: number,
    public Rating?: string,
    public Genre?: string,
    public Year?: number
  ) {}
}

export class SearchResultsModel {
  constructor(
    public Page: number,
    public Total: number,
    public Schedules?: SearchResultItemModel[]
  ) {}
}
