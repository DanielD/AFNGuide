using System.Collections;

namespace AfnGuideAPI.ViewModels
{
    public class Promo
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public bool IsPromoB { get; set; }
        public PromoSchedules Schedules { get; set; } = new();
    }

    public class PromoSchedules : IList<PromoSchedule>, ICollection<PromoSchedule>
    {
        private readonly List<PromoSchedule> innerList;

        public PromoSchedules()
        {
            innerList = new();
        }

        public PromoSchedules(IEnumerable<PromoSchedule> collection)
        {
            innerList = new();
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        #region IList Implementation

        public PromoSchedule this[int index] { get => ((IList<PromoSchedule>)innerList)[index]; set => ((IList<PromoSchedule>)innerList)[index] = value; }

        public int IndexOf(PromoSchedule item)
        {
            return ((IList<PromoSchedule>)innerList).IndexOf(item);
        }

        public void Insert(int index, PromoSchedule item)
        {
            ((IList<PromoSchedule>)innerList).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<PromoSchedule>)innerList).RemoveAt(index);
        }

        #endregion

        #region ICollection Implementation

        public int Count => ((ICollection<PromoSchedule>)innerList).Count;

        public bool IsReadOnly => ((ICollection<PromoSchedule>)innerList).IsReadOnly;

        public void Add(PromoSchedule item)
        {
            if (innerList.Any(s 
                => s.TimeZoneId == item.TimeZoneId 
                && s.Date == item.Date 
                && s.Channel == item.Channel))
            {
                return;
            }

            ((ICollection<PromoSchedule>)innerList).Add(item);
        }

        public void Clear()
        {
            ((ICollection<PromoSchedule>)innerList).Clear();
        }

        public bool Contains(PromoSchedule item)
        {
            return ((ICollection<PromoSchedule>)innerList).Contains(item);
        }

        public void CopyTo(PromoSchedule[] array, int arrayIndex)
        {
            ((ICollection<PromoSchedule>)innerList).CopyTo(array, arrayIndex);
        }

        public bool Remove(PromoSchedule item)
        {
            return ((ICollection<PromoSchedule>)innerList).Remove(item);
        }

        #endregion

        #region IEnumerable Implementation

        public IEnumerator<PromoSchedule> GetEnumerator()
        {
            return ((IEnumerable<PromoSchedule>)innerList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)innerList).GetEnumerator();
        }

        #endregion
    }

    public class PromoSchedule
    {
        public string? Channel { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public int? TimeZoneId { get; set; }
    }
}
