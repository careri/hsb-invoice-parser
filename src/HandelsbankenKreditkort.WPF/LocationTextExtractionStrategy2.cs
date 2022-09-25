using iText.IO.Util;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Text;

namespace HandelsbankenKreditkort
{
    public static class DicExt
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key)
        {
            TValue result = default(TValue);
            if (key != null)
            {
                col.TryGetValue(key, out result);
            }
            return result;
        }
        public static TValue Put<TKey, TValue>(this IDictionary<TKey, TValue> col, TKey key, TValue value)
        {
            TValue result = col.Get(key);
            col[key] = value;
            return result;
        }
    }

    /// <summary>
    /// Copy of LocationTextExtractionStrategy. Just changes the delimiter from space to ;;
    /// </summary>
    public class LocationTextExtractionStrategy2 : ITextExtractionStrategy, IEventListener
    {
        public interface ITextChunkLocationStrategy
        {
            LocationTextExtractionStrategy2.ITextChunkLocation CreateLocation(TextRenderInfo renderInfo, LineSegment baseline);
        }
        private sealed class _ITextChunkLocationStrategy_88 : LocationTextExtractionStrategy2.ITextChunkLocationStrategy
        {
            public LocationTextExtractionStrategy2.ITextChunkLocation CreateLocation(TextRenderInfo renderInfo, LineSegment baseline)
            {
                return new LocationTextExtractionStrategy2.TextChunkLocationDefaultImp(baseline.GetStartPoint(), baseline.GetEndPoint(), renderInfo.GetSingleSpaceWidth());
            }
        }
        public interface ITextChunkLocation : IComparable<LocationTextExtractionStrategy2.ITextChunkLocation>
        {
            float DistParallelEnd();
            float DistParallelStart();
            int DistPerpendicular();
            float GetCharSpaceWidth();
            Vector GetEndLocation();
            Vector GetStartLocation();
            int OrientationMagnitude();
            bool SameLine(LocationTextExtractionStrategy2.ITextChunkLocation @as);
            float DistanceFromEndOf(LocationTextExtractionStrategy2.ITextChunkLocation other);
            bool IsAtWordBoundary(LocationTextExtractionStrategy2.ITextChunkLocation previous);
        }
        /// <summary>Represents a chunk of text, it's orientation, and location relative to the orientation vector</summary>
        public class TextChunk : IComparable<LocationTextExtractionStrategy2.TextChunk>
        {
            /// <summary>the text of the chunk</summary>
            protected internal readonly string text;
            protected internal readonly LocationTextExtractionStrategy2.ITextChunkLocation location;
            public TextChunk(string @string, LocationTextExtractionStrategy2.ITextChunkLocation loc)
            {
                this.text = @string;
                this.location = loc;
            }
            /// <returns>the text captured by this chunk</returns>
            public virtual string GetText()
            {
                return this.text;
            }
            public virtual LocationTextExtractionStrategy2.ITextChunkLocation GetLocation()
            {
                return this.location;
            }
            /// <summary>Compares based on orientation, perpendicular distance, then parallel distance</summary>
            /// <seealso cref="!:System.IComparable&lt;T&gt;.CompareTo(System.Object)" />
            public virtual int CompareTo(LocationTextExtractionStrategy2.TextChunk rhs)
            {
                return this.location.CompareTo(rhs.location);
            }
            internal virtual void PrintDiagnostics()
            {
                Console.Out.WriteLine(string.Concat(new object[]
                {
                    "Text (@",
                    this.location.GetStartLocation(),
                    " -> ",
                    this.location.GetEndLocation(),
                    "): ",
                    this.text
                }));
                Console.Out.WriteLine("orientationMagnitude: " + this.location.OrientationMagnitude());
                Console.Out.WriteLine("distPerpendicular: " + this.location.DistPerpendicular());
                Console.Out.WriteLine("distParallel: " + this.location.DistParallelStart());
            }
            internal virtual bool SameLine(LocationTextExtractionStrategy2.TextChunk lastChunk)
            {
                return this.GetLocation().SameLine(lastChunk.GetLocation());
            }
        }
        public class TextChunkLocationDefaultImp : LocationTextExtractionStrategy2.ITextChunkLocation, IComparable<LocationTextExtractionStrategy2.ITextChunkLocation>
        {
            private static readonly LocationTextExtractionStrategy2.TextChunkLocationComparator defaultComparator = new LocationTextExtractionStrategy2.TextChunkLocationComparator();
            /// <summary>the starting location of the chunk</summary>
            private readonly Vector startLocation;
            /// <summary>the ending location of the chunk</summary>
            private readonly Vector endLocation;
            /// <summary>unit vector in the orientation of the chunk</summary>
            private readonly Vector orientationVector;
            /// <summary>the orientation as a scalar for quick sorting</summary>
            private readonly int orientationMagnitude;
            /// <summary>perpendicular distance to the orientation unit vector (i.e.</summary>
            /// <remarks>
            /// perpendicular distance to the orientation unit vector (i.e. the Y position in an unrotated coordinate system)
            /// we round to the nearest integer to handle the fuzziness of comparing floats
            /// </remarks>
            private readonly int distPerpendicular;
            /// <summary>distance of the start of the chunk parallel to the orientation unit vector (i.e.</summary>
            /// <remarks>distance of the start of the chunk parallel to the orientation unit vector (i.e. the X position in an unrotated coordinate system)
            ///     </remarks>
            private readonly float distParallelStart;
            /// <summary>distance of the end of the chunk parallel to the orientation unit vector (i.e.</summary>
            /// <remarks>distance of the end of the chunk parallel to the orientation unit vector (i.e. the X position in an unrotated coordinate system)
            ///     </remarks>
            private readonly float distParallelEnd;
            /// <summary>the width of a single space character in the font of the chunk</summary>
            private readonly float charSpaceWidth;
            public TextChunkLocationDefaultImp(Vector startLocation, Vector endLocation, float charSpaceWidth)
            {
                this.startLocation = startLocation;
                this.endLocation = endLocation;
                this.charSpaceWidth = charSpaceWidth;
                Vector vector = endLocation.Subtract(startLocation);
                if (vector.Length() == 0f)
                {
                    vector = new Vector(1f, 0f, 0f);
                }
                this.orientationVector = vector.Normalize();
                this.orientationMagnitude = (int)(Math.Atan2((double)this.orientationVector.Get(1), (double)this.orientationVector.Get(0)) * 1000.0);
                Vector v = new Vector(0f, 0f, 1f);
                this.distPerpendicular = (int)startLocation.Subtract(v).Cross(this.orientationVector).Get(2);
                this.distParallelStart = this.orientationVector.Dot(startLocation);
                this.distParallelEnd = this.orientationVector.Dot(endLocation);
            }
            public virtual int OrientationMagnitude()
            {
                return this.orientationMagnitude;
            }
            public virtual int DistPerpendicular()
            {
                return this.distPerpendicular;
            }
            public virtual float DistParallelStart()
            {
                return this.distParallelStart;
            }
            public virtual float DistParallelEnd()
            {
                return this.distParallelEnd;
            }
            /// <returns>the start location of the text</returns>
            public virtual Vector GetStartLocation()
            {
                return this.startLocation;
            }
            /// <returns>the end location of the text</returns>
            public virtual Vector GetEndLocation()
            {
                return this.endLocation;
            }
            /// <returns>the width of a single space character as rendered by this chunk</returns>
            public virtual float GetCharSpaceWidth()
            {
                return this.charSpaceWidth;
            }
            /// <param name="as">the location to compare to</param>
            /// <returns>true is this location is on the the same line as the other</returns>
            public virtual bool SameLine(LocationTextExtractionStrategy2.ITextChunkLocation @as)
            {
                if (this.OrientationMagnitude() != @as.OrientationMagnitude())
                {
                    return false;
                }
                float num = (float)(this.DistPerpendicular() - @as.DistPerpendicular());
                if (num == 0f)
                {
                    return true;
                }
                LineSegment lineSegment = new LineSegment(this.startLocation, this.endLocation);
                LineSegment lineSegment2 = new LineSegment(@as.GetStartLocation(), @as.GetEndLocation());
                return Math.Abs(num) <= 2f && (lineSegment.GetLength() == 0f || lineSegment2.GetLength() == 0f);
            }
            /// <summary>
            /// Computes the distance between the end of 'other' and the beginning of this chunk
            /// in the direction of this chunk's orientation vector.
            /// </summary>
            /// <remarks>
            /// Computes the distance between the end of 'other' and the beginning of this chunk
            /// in the direction of this chunk's orientation vector.  Note that it's a bad idea
            /// to call this for chunks that aren't on the same line and orientation, but we don't
            /// explicitly check for that condition for performance reasons.
            /// </remarks>
            /// <param name="other" />
            /// <returns>the number of spaces between the end of 'other' and the beginning of this chunk</returns>
            public virtual float DistanceFromEndOf(LocationTextExtractionStrategy2.ITextChunkLocation other)
            {
                return this.DistParallelStart() - other.DistParallelEnd();
            }
            public virtual bool IsAtWordBoundary(LocationTextExtractionStrategy2.ITextChunkLocation previous)
            {
                if (this.GetCharSpaceWidth() < 0.1f)
                {
                    return false;
                }
                if (this.startLocation.Equals(this.endLocation) || previous.GetEndLocation().Equals(previous.GetStartLocation()))
                {
                    return false;
                }
                float num = this.DistanceFromEndOf(previous);
                return num < -this.GetCharSpaceWidth() || num > this.GetCharSpaceWidth() / 2f;
            }
            public virtual int CompareTo(LocationTextExtractionStrategy2.ITextChunkLocation other)
            {
                return LocationTextExtractionStrategy2.TextChunkLocationDefaultImp.defaultComparator.Compare(this, other);
            }
        }
        private class TextChunkComparator : IComparer<LocationTextExtractionStrategy2.TextChunk>
        {
            private IComparer<LocationTextExtractionStrategy2.ITextChunkLocation> locationComparator;
            public TextChunkComparator(IComparer<LocationTextExtractionStrategy2.ITextChunkLocation> locationComparator)
            {
                this.locationComparator = locationComparator;
            }
            public virtual int Compare(LocationTextExtractionStrategy2.TextChunk o1, LocationTextExtractionStrategy2.TextChunk o2)
            {
                return this.locationComparator.Compare(o1.location, o2.location);
            }
        }
        private class TextChunkLocationComparator : IComparer<LocationTextExtractionStrategy2.ITextChunkLocation>
        {
            private bool leftToRight = true;
            public TextChunkLocationComparator()
            {
            }
            public TextChunkLocationComparator(bool leftToRight)
            {
                this.leftToRight = leftToRight;
            }
            public virtual int Compare(LocationTextExtractionStrategy2.ITextChunkLocation first, LocationTextExtractionStrategy2.ITextChunkLocation second)
            {
                if (first == second)
                {
                    return 0;
                }
                int num = JavaUtil.IntegerCompare(first.OrientationMagnitude(), second.OrientationMagnitude());
                if (num != 0)
                {
                    return num;
                }
                int num2 = first.DistPerpendicular() - second.DistPerpendicular();
                if (num2 != 0)
                {
                    return num2;
                }
                if (!this.leftToRight)
                {
                    return -JavaUtil.FloatCompare(first.DistParallelEnd(), second.DistParallelEnd());
                }
                return JavaUtil.FloatCompare(first.DistParallelStart(), second.DistParallelStart());
            }
        }
        private class TextChunkMarks
        {
            internal IList<LocationTextExtractionStrategy2.TextChunk> preceding = new List<LocationTextExtractionStrategy2.TextChunk>();
            internal IList<LocationTextExtractionStrategy2.TextChunk> succeeding = new List<LocationTextExtractionStrategy2.TextChunk>();
        }
        private const float DIACRITICAL_MARKS_ALLOWED_VERTICAL_DEVIATION = 2f;
        /// <summary>set to true for debugging</summary>
        private static bool DUMP_STATE;
        /// <summary>a summary of all found text</summary>
        private readonly IList<LocationTextExtractionStrategy2.TextChunk> locationalResult = new List<LocationTextExtractionStrategy2.TextChunk>();
        private readonly LocationTextExtractionStrategy2.ITextChunkLocationStrategy tclStrat;
        private bool useActualText;
        private bool rightToLeftRunDirection;
        private TextRenderInfo lastTextRenderInfo;
        /// <summary>Creates a new text extraction renderer.</summary>
        public LocationTextExtractionStrategy2() : this(new LocationTextExtractionStrategy2._ITextChunkLocationStrategy_88())
        {
        }
        /// <summary>
        /// Creates a new text extraction renderer, with a custom strategy for
        /// creating new TextChunkLocation objects based on the input of the
        /// TextRenderInfo.
        /// </summary>
        /// <param name="strat">the custom strategy</param>
        public LocationTextExtractionStrategy2(LocationTextExtractionStrategy2.ITextChunkLocationStrategy strat)
        {
            this.tclStrat = strat;
        }
        /// <summary>
        /// Changes the behavior of text extraction so that if the parameter is set to
        /// <see langword="true" />
        /// ,
        /// /ActualText marked content property will be used instead of raw decoded bytes.
        /// Beware: the logic is not stable yet.
        /// </summary>
        /// <param name="useActualText">true to use /ActualText, false otherwise</param>
        /// <returns>this object</returns>
        public virtual LocationTextExtractionStrategy2 SetUseActualText(bool useActualText)
        {
            this.useActualText = useActualText;
            return this;
        }
        /// <summary>Sets if text flows from left to right or from right to left.</summary>
        /// <remarks>
        /// Sets if text flows from left to right or from right to left.
        /// Call this method with <code>true</code> argument for extracting Arabic, Hebrew or other
        /// text with right-to-left writing direction.
        /// </remarks>
        /// <param name="rightToLeftRunDirection">value specifying whether the direction should be right to left</param>
        /// <returns>this object</returns>
        public virtual LocationTextExtractionStrategy2 SetRightToLeftRunDirection(bool rightToLeftRunDirection)
        {
            this.rightToLeftRunDirection = rightToLeftRunDirection;
            return this;
        }
        /// <summary>
        /// Gets the value of the property which determines if /ActualText will be used when extracting
        /// the text
        /// </summary>
        /// <returns>true if /ActualText value is used, false otherwise</returns>
        public virtual bool IsUseActualText()
        {
            return this.useActualText;
        }
        public virtual void EventOccurred(IEventData data, EventType type)
        {
            if (type.Equals(EventType.RENDER_TEXT))
            {
                TextRenderInfo textRenderInfo = (TextRenderInfo)data;
                LineSegment lineSegment = textRenderInfo.GetBaseline();
                if (textRenderInfo.GetRise() != 0f)
                {
                    Matrix m = new Matrix(0f, -textRenderInfo.GetRise());
                    lineSegment = lineSegment.TransformBy(m);
                }
                if (this.useActualText)
                {
                    CanvasTag canvasTag = (this.lastTextRenderInfo != null) ? this.FindLastTagWithActualText(this.lastTextRenderInfo.GetCanvasTagHierarchy()) : null;
                    if (canvasTag != null && canvasTag == this.FindLastTagWithActualText(textRenderInfo.GetCanvasTagHierarchy()))
                    {
                        LocationTextExtractionStrategy2.TextChunk textChunk = this.locationalResult[this.locationalResult.Count - 1];
                        Vector startPoint = new Vector(Math.Min(textChunk.GetLocation().GetStartLocation().Get(0), lineSegment.GetStartPoint().Get(0)), Math.Min(textChunk.GetLocation().GetStartLocation().Get(1), lineSegment.GetStartPoint().Get(1)), Math.Min(textChunk.GetLocation().GetStartLocation().Get(2), lineSegment.GetStartPoint().Get(2)));
                        Vector endPoint = new Vector(Math.Max(textChunk.GetLocation().GetEndLocation().Get(0), lineSegment.GetEndPoint().Get(0)), Math.Max(textChunk.GetLocation().GetEndLocation().Get(1), lineSegment.GetEndPoint().Get(1)), Math.Max(textChunk.GetLocation().GetEndLocation().Get(2), lineSegment.GetEndPoint().Get(2)));
                        LocationTextExtractionStrategy2.TextChunk value = new LocationTextExtractionStrategy2.TextChunk(textChunk.GetText(), this.tclStrat.CreateLocation(textRenderInfo, new LineSegment(startPoint, endPoint)));
                        this.locationalResult[this.locationalResult.Count - 1] = value;
                    }
                    else
                    {
                        string actualText = textRenderInfo.GetActualText();
                        LocationTextExtractionStrategy2.TextChunk item = new LocationTextExtractionStrategy2.TextChunk((actualText != null) ? actualText : textRenderInfo.GetText(), this.tclStrat.CreateLocation(textRenderInfo, lineSegment));
                        this.locationalResult.Add(item);
                    }
                }
                else
                {
                    LocationTextExtractionStrategy2.TextChunk item2 = new LocationTextExtractionStrategy2.TextChunk(textRenderInfo.GetText(), this.tclStrat.CreateLocation(textRenderInfo, lineSegment));
                    this.locationalResult.Add(item2);
                }
                this.lastTextRenderInfo = textRenderInfo;
            }
        }
        public virtual ICollection<EventType> GetSupportedEvents()
        {
            return null;
        }
        public virtual string GetResultantText()
        {
            if (LocationTextExtractionStrategy2.DUMP_STATE)
            {
                this.DumpState();
            }
            IList<LocationTextExtractionStrategy2.TextChunk> list = new List<LocationTextExtractionStrategy2.TextChunk>(this.locationalResult);
            this.SortWithMarks(list);
            StringBuilder stringBuilder = new StringBuilder();
            LocationTextExtractionStrategy2.TextChunk textChunk = null;
            foreach (LocationTextExtractionStrategy2.TextChunk current in list)
            {
                if (textChunk == null)
                {
                    stringBuilder.Append(current.text);
                }
                else
                {
                    if (current.SameLine(textChunk))
                    {
                        if (this.IsChunkAtWordBoundary(current, textChunk) && !this.StartsWithSpace(current.text) && !this.EndsWithSpace(textChunk.text))
                        {
                            stringBuilder.Append(";;");
                        }
                        stringBuilder.Append(current.text);
                    }
                    else
                    {
                        stringBuilder.Append('\n');
                        stringBuilder.Append(current.text);
                    }
                }
                textChunk = current;
            }
            return stringBuilder.ToString();
        }
        /// <summary>Determines if a space character should be inserted between a previous chunk and the current chunk.
        ///     </summary>
        /// <remarks>
        /// Determines if a space character should be inserted between a previous chunk and the current chunk.
        /// This method is exposed as a callback so subclasses can fine time the algorithm for determining whether a space should be inserted or not.
        /// By default, this method will insert a space if the there is a gap of more than half the font space character width between the end of the
        /// previous chunk and the beginning of the current chunk.  It will also indicate that a space is needed if the starting point of the new chunk
        /// appears *before* the end of the previous chunk (i.e. overlapping text).
        /// </remarks>
        /// <param name="chunk">the new chunk being evaluated</param>
        /// <param name="previousChunk">the chunk that appeared immediately before the current chunk</param>
        /// <returns>true if the two chunks represent different words (i.e. should have a space between them).  False otherwise.
        ///     </returns>
        protected internal virtual bool IsChunkAtWordBoundary(LocationTextExtractionStrategy2.TextChunk chunk, LocationTextExtractionStrategy2.TextChunk previousChunk)
        {
            return chunk.GetLocation().IsAtWordBoundary(previousChunk.GetLocation());
        }
        /// <summary>Checks if the string starts with a space character, false if the string is empty or starts with a non-space character.
        ///     </summary>
        /// <param name="str">the string to be checked</param>
        /// <returns>true if the string starts with a space character, false if the string is empty or starts with a non-space character
        ///     </returns>
        private bool StartsWithSpace(string str)
        {
            return str.Length != 0 && str[0] == ' ';
        }
        /// <summary>Checks if the string ends with a space character, false if the string is empty or ends with a non-space character
        ///     </summary>
        /// <param name="str">the string to be checked</param>
        /// <returns>true if the string ends with a space character, false if the string is empty or ends with a non-space character
        ///     </returns>
        private bool EndsWithSpace(string str)
        {
            return str.Length != 0 && str[str.Length - 1] == ' ';
        }
        /// <summary>Used for debugging only</summary>
        private void DumpState()
        {
            foreach (LocationTextExtractionStrategy2.TextChunk current in this.locationalResult)
            {
                current.PrintDiagnostics();
                Console.Out.WriteLine();
            }
        }
        private CanvasTag FindLastTagWithActualText(IList<CanvasTag> canvasTagHierarchy)
        {
            CanvasTag result = null;
            foreach (CanvasTag current in canvasTagHierarchy)
            {
                if (current.GetActualText() != null)
                {
                    result = current;
                    break;
                }
            }
            return result;
        }
        private void SortWithMarks(IList<LocationTextExtractionStrategy2.TextChunk> textChunks)
        {
            IDictionary<LocationTextExtractionStrategy2.TextChunk, LocationTextExtractionStrategy2.TextChunkMarks> col = new Dictionary<LocationTextExtractionStrategy2.TextChunk, LocationTextExtractionStrategy2.TextChunkMarks>();
            IList<LocationTextExtractionStrategy2.TextChunk> list = new List<LocationTextExtractionStrategy2.TextChunk>();
            for (int i = 0; i < textChunks.Count; i++)
            {
                LocationTextExtractionStrategy2.ITextChunkLocation location = textChunks[i].GetLocation();
                if (location.GetStartLocation().Equals(location.GetEndLocation()))
                {
                    bool flag = false;
                    for (int j = 0; j < textChunks.Count; j++)
                    {
                        if (i != j)
                        {
                            LocationTextExtractionStrategy2.ITextChunkLocation location2 = textChunks[j].GetLocation();
                            if (!location2.GetStartLocation().Equals(location2.GetEndLocation()) && this.ContainsMark(location2, location))
                            {
                                LocationTextExtractionStrategy2.TextChunkMarks textChunkMarks = col.Get(textChunks[j]);
                                if (textChunkMarks == null)
                                {
                                    textChunkMarks = new LocationTextExtractionStrategy2.TextChunkMarks();
                                    col.Put(textChunks[j], textChunkMarks);
                                }
                                if (i < j)
                                {
                                    textChunkMarks.preceding.Add(textChunks[i]);
                                }
                                else
                                {
                                    textChunkMarks.succeeding.Add(textChunks[i]);
                                }
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (!flag)
                    {
                        list.Add(textChunks[i]);
                    }
                }
                else
                {
                    list.Add(textChunks[i]);
                }
            }
            if (this.rightToLeftRunDirection)
            {
                JavaCollectionsUtil.Sort<LocationTextExtractionStrategy2.TextChunk>(list, new LocationTextExtractionStrategy2.TextChunkComparator(new LocationTextExtractionStrategy2.TextChunkLocationComparator(false)));
            }
            else
            {
                JavaCollectionsUtil.Sort<LocationTextExtractionStrategy2.TextChunk>(list);
            }
            textChunks.Clear();
            foreach (LocationTextExtractionStrategy2.TextChunk current in list)
            {
                LocationTextExtractionStrategy2.TextChunkMarks textChunkMarks2 = col.Get(current);
                if (textChunkMarks2 != null)
                {
                    if (!this.rightToLeftRunDirection)
                    {
                        for (int k = 0; k < textChunkMarks2.preceding.Count; k++)
                        {
                            textChunks.Add(textChunkMarks2.preceding[k]);
                        }
                    }
                    else
                    {
                        for (int l = textChunkMarks2.succeeding.Count - 1; l >= 0; l--)
                        {
                            textChunks.Add(textChunkMarks2.succeeding[l]);
                        }
                    }
                }
                textChunks.Add(current);
                if (textChunkMarks2 != null)
                {
                    if (!this.rightToLeftRunDirection)
                    {
                        for (int m = 0; m < textChunkMarks2.succeeding.Count; m++)
                        {
                            textChunks.Add(textChunkMarks2.succeeding[m]);
                        }
                    }
                    else
                    {
                        for (int n = textChunkMarks2.preceding.Count - 1; n >= 0; n--)
                        {
                            textChunks.Add(textChunkMarks2.preceding[n]);
                        }
                    }
                }
            }
        }
        private bool ContainsMark(LocationTextExtractionStrategy2.ITextChunkLocation baseLocation, LocationTextExtractionStrategy2.ITextChunkLocation markLocation)
        {
            return baseLocation.GetStartLocation().Get(0) <= markLocation.GetStartLocation().Get(0) && baseLocation.GetEndLocation().Get(0) >= markLocation.GetEndLocation().Get(0) && (float)Math.Abs(baseLocation.DistPerpendicular() - markLocation.DistPerpendicular()) <= 2f;
        }
    }
}
