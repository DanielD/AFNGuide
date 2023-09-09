export class SearchSportsResultItemModel {
  constructor(
    public Id: number,
    public SportName: string,
    public ChannelId: number,
    public IsTapeDelayed: boolean,
    public IsLive: boolean,
    public AirDateLocal: Date,
    public SportsCategoryId: number,
    public LiveDisplay: string
  ) {}
}

export class SearchSportsResultsModel {
  constructor(
    public Page: number,
    public Total: number,
    public Schedules?: SearchSportsResultItemModel[]
  ) {}
}
