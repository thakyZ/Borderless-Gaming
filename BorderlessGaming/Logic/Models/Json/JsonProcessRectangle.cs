using System.Drawing;

namespace BorderlessGaming.Logic.Models.Json
{
    public partial class JsonProcessRectangle
    {
        public static JsonProcessRectangle Empty => new (-1, -1, -1, -1);
        public static bool IsEmpty(JsonProcessRectangle rectangle) => rectangle.Height == -1 && rectangle.Width == -1 && rectangle.X == -1 && rectangle.Y == -1;


        public static JsonProcessRectangle FromProcessRectangle(ProcessRectangle rectangle)
        {
            return new JsonProcessRectangle
            {
                Height = rectangle.Height,
                Width = rectangle.Width,
                X = rectangle.X,
                Y = rectangle.Y
            };
        }


        public static ProcessRectangle ToProcessRectangle(JsonProcessRectangle rectangle)
        {
            return new ProcessRectangle
            {
                Height = rectangle.Height,
                Width = rectangle.Width,
                X = rectangle.X,
                Y = rectangle.Y
            };
        }


        public static JsonProcessRectangle ToJsonProcessRectangle(Rectangle rectangle)
        {
            return new JsonProcessRectangle
            {
                Height = rectangle.Height,
                Width = rectangle.Width,
                X = rectangle.X,
                Y = rectangle.Y
            };
        }

        public static Rectangle ToRectangle(JsonProcessRectangle pRectangle)
        {
            return new Rectangle
            {
                Height = pRectangle.Height,
                Width = pRectangle.Width,
                X = pRectangle.X,
                Y = pRectangle.Y
            };
        }
    }
}