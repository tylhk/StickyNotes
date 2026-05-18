namespace StickyNotes
{
    public class NoteData
    {
        public string Content { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public IntPtr TargetWindowHandle { get; set; }
        public string TargetWindowTitle { get; set; }
        public string TargetWindowClass { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
    }
}
