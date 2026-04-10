namespace TodoApp.Services
{
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Static helpers for extracting typed properties from plain JavaScript objects
    /// returned by JSON.parse. NScript does not support 'dynamic', so these
    /// [Script] methods access raw JS properties by name.
    /// </summary>
    public static class JsonHelper
    {
        [Script(@"return obj ? (obj.Id || '') : '';")]
        public static extern string GetId(object obj);

        [Script(@"return obj ? (obj.Title || '') : '';")]
        public static extern string GetTitle(object obj);

        [Script(@"return obj ? (obj.FolderId || '') : '';")]
        public static extern string GetFolderId(object obj);

        [Script(@"return obj ? !!obj.IsCompleted : false;")]
        public static extern bool GetIsCompleted(object obj);

        [Script(@"return obj ? !!obj.IsImportant : false;")]
        public static extern bool GetIsImportant(object obj);

        [Script(@"return obj ? !!obj.IsMyDay : false;")]
        public static extern bool GetIsMyDay(object obj);

        [Script(@"return obj ? (obj.DueDate || '') : '';")]
        public static extern string GetDueDate(object obj);

        [Script(@"return obj ? (obj.Notes || '') : '';")]
        public static extern string GetNotes(object obj);

        [Script(@"return obj ? (obj.SubTasks || []) : [];")]
        public static extern object GetSubTasks(object obj);

        [Script(@"return obj ? (obj.Name || '') : '';")]
        public static extern string GetName(object obj);

        [Script(@"return obj ? (obj.Icon || '') : '';")]
        public static extern string GetIcon(object obj);

        [Script(@"return obj ? !!obj.IsSystem : false;")]
        public static extern bool GetIsSystem(object obj);

        [Script(@"return obj ? (obj.SystemType || '') : '';")]
        public static extern string GetSystemType(object obj);

        [Script(@"return obj ? (obj.SortOrder || 0) : 0;")]
        public static extern int GetSortOrder(object obj);

        [Script(@"return json ? @:JSON.parse(json) : [];")]
        public static extern object Parse(string json);

        /// <summary>
        /// Returns the length of a native JS array, or 0 if null.
        /// </summary>
        [Script(@"return (arr && arr.length) ? arr.length : 0;")]
        public static extern int GetArrayLength(object arr);

        /// <summary>
        /// Returns the element at the given index from a native JS array.
        /// </summary>
        [Script(@"return arr ? arr[index] : null;")]
        public static extern object GetArrayItem(object arr, int index);
    }
}
