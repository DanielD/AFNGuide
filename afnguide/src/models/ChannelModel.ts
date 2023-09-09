export class ChannelModel {
  constructor(
    public Id: number,
    public Title: string,
    public ChannelNumber: number,
    public Color: string,
    public IsSplit: boolean,
    public IsSports: boolean,
    public ChannelTimeZones: ChannelTimeZoneModel[]
  ) {}
}

export class ChannelTimeZoneModel {
  constructor(
    public TimeZoneId: number,
    public StartTime: string,
    public EndTime: string
  ) {}
}
