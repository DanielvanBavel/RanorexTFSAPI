namespace RanorexTFSAPI.API
{
    public class ListItem
    {
        public int WorkItemId;
        public string FieldKey;
        public string FieldValue;

        public ListItem(int workItemId, string fieldKey, string fieldValue)
        {
            WorkItemId = workItemId;
            FieldKey = fieldKey;
            FieldValue = fieldValue;
        }
    }
}
