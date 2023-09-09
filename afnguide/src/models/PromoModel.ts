export class PromoModel {
  constructor(
    public Title: string | undefined,
    public Description: string | undefined,
    public ImageUrl: string | undefined,
    public LinkUrl: string | undefined,
    public IsPromoB: boolean,
    public Schedules: PromoScheduleModel[] | undefined
  ) {}
}

export class PromoScheduleModel {
  constructor(
    public Channel: string | undefined,
    public Date: string | undefined,
    public Time: string | undefined,
    public TimeZoneId: number | undefined
  ) {}
}
