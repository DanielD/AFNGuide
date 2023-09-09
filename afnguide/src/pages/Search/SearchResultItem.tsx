import { useContext } from "react";
import { AfnContext } from "../../modules/AfnContext";
import { Icon } from "../../components/Icon";
import DayJS from "../../plugins/react-dayjs";
import { SearchResultItemModel } from "../../models/SearchResultsModel";
import { useCookies } from "react-cookie";

function SearchResultItem(props: { item: SearchResultItemModel }) {
  const { channels } = useContext(AfnContext);
  const channel = channels.find((c) => c.Id === props.item.ChannelId)!;
  const [cookies] = useCookies(["timezone"]);
  const channelTimeZone = channel.ChannelTimeZones.find(
    (ctz) => ctz.TimeZoneId === parseInt(cookies.timezone)
  )!;

  function determineSplitChannel(
    date: Date,
    startTimeSpan: string,
    endTimeSpan: string
  ): number {
    const start = new Date(date);
    const end = new Date(date);

    if (startTimeSpan.includes(".")) {
      const startDays = startTimeSpan.split(".")[0];
      startTimeSpan = startTimeSpan.split(".")[1];
      start.setDate(start.getDate() + parseInt(startDays));
    }

    if (endTimeSpan.includes(".")) {
      const endDays = endTimeSpan.split(".")[0];
      endTimeSpan = endTimeSpan.split(".")[1];
      end.setDate(end.getDate() + parseInt(endDays));
    }

    const startTime = startTimeSpan.split(":");
    start.setHours(parseInt(startTime[0]));
    start.setMinutes(parseInt(startTime[1]));

    const endTime = endTimeSpan.split(":");
    end.setHours(parseInt(endTime[0]));
    end.setMinutes(parseInt(endTime[1]));

    if (start > end) {
      end.setDate(end.getDate() + 1);
    }

    if (date > start && date < end) {
      return 1;
    } else {
      return 0;
    }
  }

  let channelTitle = "";
  let channelColor = "";
  if (!channel.IsSplit) {
    channelTitle = channel.Title;
    channelColor = channel.Color;
  } else {
    const channelFlag = determineSplitChannel(
      props.item.AirDateLocal,
      channelTimeZone.StartTime,
      channelTimeZone.EndTime
    );
    channelTitle = channel.Title.split(",")[channelFlag];
    channelColor = channel.Color.split(",")[channelFlag];
  }

  return (
    <>
      <article className="col-lg-12 g-py-10" style={{ boxShadow: `0 0 10px ${channelColor}`}}>
        <header className="g-mb-15">
          <div className="d-lg-flex justify-content-between align-items-center">
            <div>
              <h2 className="h4 g-mb-5">
                <button className="h4 btn btn-link g-color-gray-dark-v1 u-link-v5 g-pa-0 g-ma-0">
                  {props.item.Title}
                </button>
              </h2>
              <span className="g-color-primary">{props.item.EpisodeTitle}</span>
            </div>
            <h2 className="h4 g-color-primary g-mb-5">{channelTitle}</h2>
          </div>
        </header>
        <p className="g-mb-15">{props.item.Description}</p>
        <div className="d-lg-flex justify-content-between align-items-center">
          <ul className="list-inline g-mb-10 g-mb-0--lg">
            <li className="list-inline-item g-mr-30">
              <Icon
                iconName="Clipboard2"
                className="g-pos-rel g-top-1 g-color-gray-dark-v5 g-mr-5"
                title="Director"
              />
              &nbsp;Stephen Maier
            </li>
            <li className="list-inline-item g-mr-30">
              <Icon
                iconName="Calendar2Date"
                className="g-pos-rel g-top-1 g-color-gray-dark-v5 g-mr-5"
                title="First Aired"
              />
              &nbsp;May 23, 2023
            </li>
            <li className="list-inline-item g-mr-30">
              <Icon
                iconName="StarFill"
                className="g-pos-rel g-top-1 g-color-gray-dark-v5 g-mr-5"
                title="iMDb Rating"
              />
              &nbsp;7.7/10
            </li>
          </ul>
          <div>
            <span className="g-color-primary">
              <DayJS date={props.item.AirDateLocal} format="MMM D YYYY h:mmA" />
              &nbsp;(Local Time)
            </span>
          </div>
        </div>
      </article>
      <hr className="afn-brd-color-light g-my-30 g-mx-15" />
    </>
  );
}

export default SearchResultItem;
