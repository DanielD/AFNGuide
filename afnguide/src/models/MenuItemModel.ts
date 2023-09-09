export class MenuItemModel {
  constructor(
    public Text: string,
    public Url: string,
    public Image: string,
    public Items: MenuItemModel[]
  ) {}
}
