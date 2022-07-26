using GMap.NET;
using PZ2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace PZ2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double noviX, noviY, minLat, minLon, maxLat, maxLon, deltaLat, deltaLon;

        private int zoomMax = 35;
        private int zoomCurent = 1;

        private System.Windows.Point start = new Point();
        private System.Windows.Point diffOffset = new Point();

        public static List<SwitchEntity> switches;
        public static List<SubstationEntity> substations;
        public static List<NodeEntity> nodes;
        public static List<LineEntity> lines;
        public static List<PowerEntity> entities;
        List<PointLatLng> points;

        //geometrije
        AmbientLight ambientLight;
        DirectionalLight directionalLight;
        Model3DGroup myModel3DGroup;
        GeometryModel3D myGeometryModel;
        ModelVisual3D myModelVisual3D;

        //transformacije
        Transform3DGroup transform3DGroup;
        TranslateTransform3D translateTransform3D;
        ScaleTransform3D scaleTransform3D;
        RotateTransform3D rotateTransform3D;
        AxisAngleRotation3D axisAngleRotation3D;

        //elemnti
        Point3DCollection cubes;
        Dictionary<long, GeometryModel3D> geometryModels;
        Dictionary<long, List<GeometryModel3D>> geometryLines;

        //hiting
        GeometryModel3D hitGeometry1, hitGeometry2;
        Material hitMaterial1, hitMaterial2;
        System.Windows.Point mouseposition;
        ToolTip tooltip;

        public MainWindow()
        {
            InitializeComponent();

            substations = new List<SubstationEntity>();
            nodes = new List<NodeEntity>();
            switches = new List<SwitchEntity>();
            lines = new List<LineEntity>();
            points = new List<PointLatLng>();
            entities = new List<PowerEntity>();

            myModelVisual3D = new ModelVisual3D();
            myGeometryModel = new GeometryModel3D();
            myModel3DGroup = new Model3DGroup();

            cubes = new Point3DCollection();
            geometryModels = new Dictionary<long, GeometryModel3D>();
            geometryLines = new Dictionary<long, List<GeometryModel3D>>();

            tooltip = new ToolTip();

            LoadModel();
            LoadXML();
        }

        #region load
        
        private void LoadModel() {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("Geographic.xml");

            XmlNodeList nodeList;
            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");

            foreach (XmlNode node in nodeList)
            {
                SubstationEntity sub = new SubstationEntity();
                sub.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                sub.Name = node.SelectSingleNode("Name").InnerText;
                sub.X = double.Parse(node.SelectSingleNode("X").InnerText);
                sub.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                double x, y;

                ToLatLon(sub.X, sub.Y, 34, out x, out y);

                sub.X = x;
                sub.Y = y;

                substations.Add(sub);

                entities.Add(sub);
            }

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            foreach (XmlNode node in nodeList)
            {
                NodeEntity nodeobj = new NodeEntity();
                nodeobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                nodeobj.Name = node.SelectSingleNode("Name").InnerText;
                nodeobj.X = double.Parse(node.SelectSingleNode("X").InnerText);
                nodeobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                double x, y;

                ToLatLon(nodeobj.X, nodeobj.Y, 34, out x, out y);

                nodeobj.X = x;
                nodeobj.Y = y;

                nodes.Add(nodeobj);

                entities.Add(nodeobj);

            }

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");

            foreach (XmlNode node in nodeList)
            {
                SwitchEntity switchobj = new SwitchEntity();
                switchobj.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                switchobj.Name = node.SelectSingleNode("Name").InnerText;
                switchobj.X = double.Parse(node.SelectSingleNode("X").InnerText);
                switchobj.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
                switchobj.Status = node.SelectSingleNode("Status").InnerText;

                double x, y;

                ToLatLon(switchobj.X, switchobj.Y, 34, out x, out y);

                switchobj.X = x;
                switchobj.Y = y;

                switches.Add(switchobj);

                entities.Add(switchobj);
            }

            nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");

            foreach (XmlNode node in nodeList)
            {
                LineEntity l = new LineEntity();
                l.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                l.Name = node.SelectSingleNode("Name").InnerText;
                l.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
                l.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);
                l.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                if (node.SelectSingleNode("IsUnderground").InnerText.Equals("true"))
                {
                    l.IsUnderground = true;
                }
                else
                {
                    l.IsUnderground = false;
                }
                l.LineType = node.SelectSingleNode("LineType").InnerText;
                l.R = float.Parse(node.SelectSingleNode("R").InnerText);
                l.ThermalConstantHeat = long.Parse(node.SelectSingleNode("ThermalConstantHeat").InnerText);
                l.Vertices = new List<Models.Points>();

                foreach (XmlNode pointNode in node.ChildNodes[9].ChildNodes) // 9 posto je Vertices 9. node u jednom line objektu
                {
                    Models.Points p = new Models.Points();

                    p.X = double.Parse(pointNode.SelectSingleNode("X").InnerText);
                    p.Y = double.Parse(pointNode.SelectSingleNode("Y").InnerText);

                    ToLatLon(p.X, p.Y, 34, out noviX, out noviY);

                    points.Add(new PointLatLng(noviX, noviY));

                    l.Vertices.Add(new Models.Points() { X = noviX, Y = noviY });
                }

                lines.Add(l);

            }
        }
        public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double longitude, out double latitude)
        {
            bool isNorthHemisphere = true;

            var diflat = -0.00066286966871111111111111111111111111;
            var diflon = -0.0003868060578;

            var zone = zoneUTM;
            var c_sa = 6378137.000000;
            var c_sb = 6356752.314245;
            var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
            var e2cuadrada = Math.Pow(e2, 2);
            var c = Math.Pow(c_sa, 2) / c_sb;
            var x = utmX - 500000;
            var y = isNorthHemisphere ? utmY : utmY - 10000000;

            var s = ((zone * 6.0) - 183.0);
            var lat = y / (c_sa * 0.9996);
            var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
            var a = x / v;
            var a1 = Math.Sin(2 * lat);
            var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
            var j2 = lat + (a1 / 2.0);
            var j4 = ((3 * j2) + a2) / 4.0;
            var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
            var alfa = (3.0 / 4.0) * e2cuadrada;
            var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
            var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
            var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
            var b = (y - bm) / v;
            var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
            var eps = a * (1 - (epsi / 3.0));
            var nab = (b * (1 - epsi)) + lat;
            var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
            var delt = Math.Atan(senoheps / (Math.Cos(nab)));
            var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

            longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
            latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
        }

        #endregion load

        #region draw

        private void LoadXML() {
            ambientLight = new AmbientLight();
            ambientLight.Color = Colors.White;

            directionalLight = new DirectionalLight();
            directionalLight.Color = Colors.White;
            directionalLight.Direction = new Vector3D(-10, -10, -10);

            myModel3DGroup.Children.Add(directionalLight);

            DrawMap();
            DrawElements();
            DrawLines();

            myModelVisual3D.Content = myModel3DGroup;

            viewport1.Children.Add(myModelVisual3D);
        }

        #region map

        private void DrawMap()
        {
            MeshGeometry3D myMeshGeometry3D = new MeshGeometry3D();

            //tacke mape
            Point3DCollection myPositionCollection = new Point3DCollection();
            myPositionCollection.Add(new Point3D(-400, 0, 400));
            myPositionCollection.Add(new Point3D(400, 0, 400));
            myPositionCollection.Add(new Point3D(400, 0, -400));
            myPositionCollection.Add(new Point3D(-400, 0, -400));
            myMeshGeometry3D.Positions = myPositionCollection;

            //kako ce se lepiti slika
            PointCollection myTextureCoordinatesCollection = new PointCollection();
            myTextureCoordinatesCollection.Add(new System.Windows.Point(0, 1));
            myTextureCoordinatesCollection.Add(new System.Windows.Point(1, 1));
            myTextureCoordinatesCollection.Add(new System.Windows.Point(1, 0));
            myTextureCoordinatesCollection.Add(new System.Windows.Point(0, 0));
            myMeshGeometry3D.TextureCoordinates = myTextureCoordinatesCollection;

            //definisanje trouglova
            Int32Collection myTriangleIndicesCollection = new Int32Collection();
            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(1);
            myTriangleIndicesCollection.Add(2);

            myTriangleIndicesCollection.Add(0);
            myTriangleIndicesCollection.Add(2);
            myTriangleIndicesCollection.Add(3);
            myMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

            myGeometryModel.Geometry = myMeshGeometry3D;

            //od kog je matrijala
            DiffuseMaterial material = new DiffuseMaterial();

            BitmapImage image = new BitmapImage(new Uri("C:\\Users\\Mihajlo\\Desktop\\Grafika_Projekat_2\\Z2\\PZ2\\map.jpg", UriKind.Absolute));

            material.Brush = new ImageBrush(image);

            myGeometryModel.Material = material;

            transform3DGroup = new Transform3DGroup();

            translateTransform3D = new TranslateTransform3D();

            translateTransform3D.OffsetX = 0;
            translateTransform3D.OffsetY = 0;
            translateTransform3D.OffsetZ = 0;

            scaleTransform3D = new ScaleTransform3D();

            scaleTransform3D.ScaleX = 1;
            scaleTransform3D.ScaleY = 1;
            scaleTransform3D.ScaleZ = 1;

            rotateTransform3D = new RotateTransform3D();

            rotateTransform3D.CenterX = 0;
            rotateTransform3D.CenterY = 0;
            rotateTransform3D.CenterZ = 0;

            axisAngleRotation3D = new AxisAngleRotation3D();

            axisAngleRotation3D.Angle = 0;
            axisAngleRotation3D.Axis = new Vector3D(0, 1, 0);

            rotateTransform3D.Rotation = axisAngleRotation3D;

            transform3DGroup.Children.Add(translateTransform3D);
            transform3DGroup.Children.Add(scaleTransform3D);
            transform3DGroup.Children.Add(rotateTransform3D);

            myGeometryModel.Transform = transform3DGroup;

            myModel3DGroup.Children.Add(myGeometryModel);
        }

        #endregion map

        #region elements

        private void DrawElements()
        {
            minLat = 45.2325;
            minLon = 19.793909;
            maxLat = 45.277031;
            maxLon = 19.894459;

            deltaLat = maxLat - minLat;
            deltaLon = maxLon - minLon;

            DrawSubstations();
            DrawNodes();
            DrawSwitches();

        }

        private void DrawSubstations()
        {
            foreach (SubstationEntity entity in substations)
            {
                MeshGeometry3D substationMeshGeometry3D = new MeshGeometry3D();

                int x = Convert.ToInt32(LonToX(entity.X));
                int y = 0;
                int z = Convert.ToInt32(LatToZ(entity.Y));

                if (x < -400 || x > 400 || z < -400 || z > 400)
                    continue;

                while (CubeIntersection(new Point3D(x, y, z)))
                {
                    y += 10;
                }

                cubes.Add(new Point3D(x, y, z));

                Point3DCollection myPositionCollection = new Point3DCollection();
                myPositionCollection.Add(new Point3D(x - 5, y, z + 5));
                myPositionCollection.Add(new Point3D(x + 5, y, z + 5));
                myPositionCollection.Add(new Point3D(x + 5, y, z - 5));
                myPositionCollection.Add(new Point3D(x - 5, y, z - 5));

                myPositionCollection.Add(new Point3D(x - 5, y + 10, z + 5));
                myPositionCollection.Add(new Point3D(x + 5, y + 10, z + 5));
                myPositionCollection.Add(new Point3D(x + 5, y + 10, z - 5));
                myPositionCollection.Add(new Point3D(x - 5, y + 10, z - 5));

                substationMeshGeometry3D.Positions = myPositionCollection;

                Int32Collection myTriangleIndicesCollection = new Int32Collection();
                //donja strana
                myTriangleIndicesCollection.Add(0);
                myTriangleIndicesCollection.Add(1);
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(0);
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(3);
                //gornja strana
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(5);
                myTriangleIndicesCollection.Add(6);
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(6);
                myTriangleIndicesCollection.Add(7);
                //prednja strana
                myTriangleIndicesCollection.Add(0);
                myTriangleIndicesCollection.Add(1);
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(1);
                myTriangleIndicesCollection.Add(5);
                myTriangleIndicesCollection.Add(4);
                //zadnja strana
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(3);
                myTriangleIndicesCollection.Add(7);
                myTriangleIndicesCollection.Add(7);
                myTriangleIndicesCollection.Add(6);
                myTriangleIndicesCollection.Add(2);
                //leva strana
                myTriangleIndicesCollection.Add(3);
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(7);
                myTriangleIndicesCollection.Add(0);
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(3);
                //desnas strana
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(6);
                myTriangleIndicesCollection.Add(5);
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(5);
                myTriangleIndicesCollection.Add(1);

                substationMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

                GeometryModel3D substationGeometryModel = new GeometryModel3D();

                substationGeometryModel.Geometry = substationMeshGeometry3D;

                DiffuseMaterial material = new DiffuseMaterial();

                material.Brush = Brushes.White;

                substationGeometryModel.Material = material;

                substationGeometryModel.Transform = transform3DGroup;

                geometryModels.Add(entity.Id, substationGeometryModel);

                myModel3DGroup.Children.Add(substationGeometryModel);

            }
        }

        private void DrawNodes()
        {
            foreach (NodeEntity entity in nodes)
            {
                MeshGeometry3D substationMeshGeometry3D = new MeshGeometry3D();

                int x = Convert.ToInt32(LonToX(entity.X));
                int y = 0;
                int z = Convert.ToInt32(LatToZ(entity.Y));

                if (x < -400 || x > 400 || z < -400 || z > 400)
                    continue;

                while (CubeIntersection(new Point3D(x, y, z)))
                {
                    y += 10;
                }

                cubes.Add(new Point3D(x, y, z));

                Point3DCollection myPositionCollection = new Point3DCollection();
                myPositionCollection.Add(new Point3D(x - 5, y, z + 5));
                myPositionCollection.Add(new Point3D(x + 5, y, z + 5));
                myPositionCollection.Add(new Point3D(x + 5, y, z - 5));
                myPositionCollection.Add(new Point3D(x - 5, y, z - 5));

                myPositionCollection.Add(new Point3D(x - 5, y + 10, z + 5));
                myPositionCollection.Add(new Point3D(x + 5, y + 10, z + 5));
                myPositionCollection.Add(new Point3D(x + 5, y + 10, z - 5));
                myPositionCollection.Add(new Point3D(x - 5, y + 10, z - 5));

                substationMeshGeometry3D.Positions = myPositionCollection;

                Int32Collection myTriangleIndicesCollection = new Int32Collection();
                //donja strana
                myTriangleIndicesCollection.Add(0);
                myTriangleIndicesCollection.Add(1);
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(0);
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(3);
                //gornja strana
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(5);
                myTriangleIndicesCollection.Add(6);
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(6);
                myTriangleIndicesCollection.Add(7);
                //prednja strana
                myTriangleIndicesCollection.Add(0);
                myTriangleIndicesCollection.Add(1);
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(1);
                myTriangleIndicesCollection.Add(5);
                myTriangleIndicesCollection.Add(4);
                //zadnja strana
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(3);
                myTriangleIndicesCollection.Add(7);
                myTriangleIndicesCollection.Add(7);
                myTriangleIndicesCollection.Add(6);
                myTriangleIndicesCollection.Add(2);
                //leva strana
                myTriangleIndicesCollection.Add(3);
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(7);
                myTriangleIndicesCollection.Add(0);
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(3);
                //desnas strana
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(6);
                myTriangleIndicesCollection.Add(5);
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(5);
                myTriangleIndicesCollection.Add(1);

                substationMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

                GeometryModel3D substationGeometryModel = new GeometryModel3D();

                substationGeometryModel.Geometry = substationMeshGeometry3D;

                DiffuseMaterial material = new DiffuseMaterial();

                material.Brush = Brushes.Blue;

                substationGeometryModel.Material = material;

                substationGeometryModel.Transform = transform3DGroup;

                geometryModels.Add(entity.Id, substationGeometryModel);

                myModel3DGroup.Children.Add(substationGeometryModel);

            }
        }

        private void DrawSwitches()
        {
            foreach (SwitchEntity entity in switches)
            {
                MeshGeometry3D substationMeshGeometry3D = new MeshGeometry3D();

                int x = Convert.ToInt32(LonToX(entity.X));
                int y = 0;
                int z = Convert.ToInt32(LatToZ(entity.Y));

                if (x < -400 || x > 400 || z < -400 || z > 400)
                    continue;

                while (CubeIntersection(new Point3D(x, y, z)))
                {
                    y += 10;
                }

                cubes.Add(new Point3D(x, y, z));

                Point3DCollection myPositionCollection = new Point3DCollection();
                myPositionCollection.Add(new Point3D(x - 5, y, z + 5));
                myPositionCollection.Add(new Point3D(x + 5, y, z + 5));
                myPositionCollection.Add(new Point3D(x + 5, y, z - 5));
                myPositionCollection.Add(new Point3D(x - 5, y, z - 5));

                myPositionCollection.Add(new Point3D(x - 5, y + 10, z + 5));
                myPositionCollection.Add(new Point3D(x + 5, y + 10, z + 5));
                myPositionCollection.Add(new Point3D(x + 5, y + 10, z - 5));
                myPositionCollection.Add(new Point3D(x - 5, y + 10, z - 5));

                substationMeshGeometry3D.Positions = myPositionCollection;

                Int32Collection myTriangleIndicesCollection = new Int32Collection();
                //donja strana
                myTriangleIndicesCollection.Add(0);
                myTriangleIndicesCollection.Add(1);
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(0);
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(3);
                //gornja strana
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(5);
                myTriangleIndicesCollection.Add(6);
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(6);
                myTriangleIndicesCollection.Add(7);
                //prednja strana
                myTriangleIndicesCollection.Add(0);
                myTriangleIndicesCollection.Add(1);
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(1);
                myTriangleIndicesCollection.Add(5);
                myTriangleIndicesCollection.Add(4);
                //zadnja strana
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(3);
                myTriangleIndicesCollection.Add(7);
                myTriangleIndicesCollection.Add(7);
                myTriangleIndicesCollection.Add(6);
                myTriangleIndicesCollection.Add(2);
                //leva strana
                myTriangleIndicesCollection.Add(3);
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(7);
                myTriangleIndicesCollection.Add(0);
                myTriangleIndicesCollection.Add(4);
                myTriangleIndicesCollection.Add(3);
                //desnas strana
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(6);
                myTriangleIndicesCollection.Add(5);
                myTriangleIndicesCollection.Add(2);
                myTriangleIndicesCollection.Add(5);
                myTriangleIndicesCollection.Add(1);

                substationMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

                GeometryModel3D substationGeometryModel = new GeometryModel3D();

                substationGeometryModel.Geometry = substationMeshGeometry3D;

                DiffuseMaterial material = new DiffuseMaterial();

                material.Brush = Brushes.Orange;

                substationGeometryModel.Material = material;

                substationGeometryModel.Transform = transform3DGroup;

                geometryModels.Add(entity.Id, substationGeometryModel);

                myModel3DGroup.Children.Add(substationGeometryModel);

            }
        }

        private bool CubeIntersection(Point3D point)
        {
            foreach (Point3D cube in cubes)
            {
                //ako nisu na istom nivou ni ne proveravaj
                if (cube.Y != point.Y)
                    continue;

                Rect newRectangle = new Rect(new System.Windows.Point(point.X - 5, point.Z - 5),
                                            new System.Windows.Point(point.X + 5, point.Z + 5));

                Rect existingRectangle = new Rect(new System.Windows.Point(cube.X - 5, cube.Z - 5),
                                            new System.Windows.Point(cube.X + 5, cube.Z + 5));

                if (!Rect.Intersect(newRectangle, existingRectangle).IsEmpty)
                    return true;

            }

            return false;
        }

        private double LatToZ(double lat)
        {
            return 400 - ((lat - minLat) * 800) / deltaLat;
        }

        private double LonToX(double lon)
        {
            return ((lon - minLon) * 800) / deltaLon - 400;
        }

        private void DrawLines()
        {
            List<Models.Points> linePoints = new List<Models.Points>();

            foreach (LineEntity entity in lines)
            {
                linePoints = new List<Models.Points>();

                PowerEntity p1 = entities.FirstOrDefault(e => e.Id == entity.FirstEnd);
                PowerEntity p2 = entities.FirstOrDefault(e => e.Id == entity.SecondEnd);

                if (p1 == null || p2 == null)
                    continue;

                if (LonToX(p1.X) < -400 || LonToX(p1.X) > 400)
                    continue;

                if (LatToZ(p1.Y) < -400 || LatToZ(p1.Y) > 400)
                    continue;

                if (LonToX(p2.X) < -400 || LonToX(p2.X) > 400)
                    continue;

                if (LatToZ(p2.Y) < -400 || LatToZ(p2.Y) > 400)
                    continue;

                Models.Points firstEnd = new Models.Points()
                {
                    X = p1.X,
                    Y = p1.Y
                };

                Models.Points secondEnd = new Models.Points()
                {
                    X = p2.X,
                    Y = p2.Y
                };

                linePoints.Add(firstEnd);

                foreach (Models.Points point in entity.Vertices)
                {
                    linePoints.Add(point);
                }

                linePoints.Add(secondEnd);

                geometryLines.Add(entity.Id, new List<GeometryModel3D>());

                for (int i = 0; i < linePoints.Count - 1; i++)
                {
                    Models.Points point1 = linePoints[i];
                    Models.Points point2 = linePoints[i + 1];

                    MeshGeometry3D substationMeshGeometry3D = new MeshGeometry3D();

                    if (point1.Y > point2.Y)
                    {
                        var temp = point1;
                        point1 = point2;
                        point2 = temp;
                    }

                    double x1 = LonToX(point1.X);
                    double y1 = 0;
                    double z1 = LatToZ(point1.Y);

                    double x2 = LonToX(point2.X);
                    double y2 = 0;
                    double z2 = LatToZ(point2.Y);

                    double radians = Math.Atan((point2.Y - point1.Y) / (point2.X - point1.X));
                    double angle = radians * (180 / Math.PI);
                    double movementX = Math.Abs(Math.Sin(radians));
                    double movementZ = Math.Abs(Math.Cos(radians));

                    Point3DCollection myPositionCollection = new Point3DCollection();

                    if (Math.Tan(radians) > 0)
                    {
                        myPositionCollection.Add(new Point3D(x1 - 2 * movementX, y1, z1 - 2 * movementZ));
                        myPositionCollection.Add(new Point3D(x1 + 2 * movementX, y1, z1 + 2 * movementZ));
                        myPositionCollection.Add(new Point3D(x1, y1 + 3, z1));

                        myPositionCollection.Add(new Point3D(x2 - 2 * movementX, y2, z2 - 2 * movementZ));
                        myPositionCollection.Add(new Point3D(x2 + 2 * movementX, y2, z2 + 2 * movementZ));
                        myPositionCollection.Add(new Point3D(x2, y2 + 3, z2));
                    }
                    else
                    {
                        myPositionCollection.Add(new Point3D(x1 - 2 * movementX, y1, z1 + 2 * movementZ));
                        myPositionCollection.Add(new Point3D(x1 + 2 * movementX, y1, z1 - 2 * movementZ));
                        myPositionCollection.Add(new Point3D(x1, y1 + 3, z1));

                        myPositionCollection.Add(new Point3D(x2 - 2 * movementX, y2, z2 + 2 * movementZ));
                        myPositionCollection.Add(new Point3D(x2 + 2 * movementX, y2, z2 - 2 * movementZ));
                        myPositionCollection.Add(new Point3D(x2, y2 + 3, z2));
                    }

                    substationMeshGeometry3D.Positions = myPositionCollection;

                    Int32Collection myTriangleIndicesCollection = new Int32Collection();
                    myTriangleIndicesCollection.Add(0);
                    myTriangleIndicesCollection.Add(1);
                    myTriangleIndicesCollection.Add(2);

                    myTriangleIndicesCollection.Add(5);
                    myTriangleIndicesCollection.Add(4);
                    myTriangleIndicesCollection.Add(3);

                    myTriangleIndicesCollection.Add(1);
                    myTriangleIndicesCollection.Add(5);
                    myTriangleIndicesCollection.Add(2);
                    myTriangleIndicesCollection.Add(1);
                    myTriangleIndicesCollection.Add(4);
                    myTriangleIndicesCollection.Add(5);

                    myTriangleIndicesCollection.Add(0);
                    myTriangleIndicesCollection.Add(5);
                    myTriangleIndicesCollection.Add(3);
                    myTriangleIndicesCollection.Add(0);
                    myTriangleIndicesCollection.Add(2);
                    myTriangleIndicesCollection.Add(5);

                    substationMeshGeometry3D.TriangleIndices = myTriangleIndicesCollection;

                    GeometryModel3D substationGeometryModel = new GeometryModel3D();

                    substationGeometryModel.Geometry = substationMeshGeometry3D;

                    DiffuseMaterial material = new DiffuseMaterial();

                    if (entity.ConductorMaterial == "Steel")
                        material.Brush = Brushes.Yellow;
                    else if (entity.ConductorMaterial == "Other")
                        material.Brush = Brushes.Pink;
                    else if (entity.ConductorMaterial == "Acsr")
                        material.Brush = Brushes.Black;
                    else if (entity.ConductorMaterial == "Copper")
                        material.Brush = Brushes.Brown;

                    substationGeometryModel.Material = material;

                    substationGeometryModel.Transform = transform3DGroup;

                    geometryLines[entity.Id].Add(substationGeometryModel);

                    myModel3DGroup.Children.Add(substationGeometryModel);

                }

            }
        }

        #endregion elements

        #endregion draw

        #region MouseAction
        private void viewport1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewport1.ReleaseMouseCapture();
        }
        private void viewport1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            System.Windows.Point p = e.MouseDevice.GetPosition(this);
            double scaleX=1;
            double scaleZ=1;

            if (e.Delta > 0 && zoomCurent < zoomMax) {
                scaleX = scaleTransform3D.ScaleX + 0.1;
                scaleZ = scaleTransform3D.ScaleZ + 0.1;
                zoomCurent++;
                scaleTransform3D.ScaleX = scaleX;
                scaleTransform3D.ScaleZ = scaleZ;
            } else if (e.Delta <= 0 && zoomCurent > 1) {
                scaleX = scaleTransform3D.ScaleX - 0.1;
                scaleZ = scaleTransform3D.ScaleZ - 0.1;
                zoomCurent--;
                scaleTransform3D.ScaleX = scaleX;
                scaleTransform3D.ScaleZ = scaleZ;
            }
        }
        private void viewport1_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewport1.IsMouseCaptured && e.MiddleButton == MouseButtonState.Released)
            {
                System.Windows.Point end = e.GetPosition(this);
                double offsetX = end.X - start.X;
                double offsetY = end.Y - start.Y;
                double w = this.Width;
                double h = this.Height;
                double translateX = (offsetX * 100) / w;
                double translateY = (offsetY * 100) / h;
                translateTransform3D.OffsetX = diffOffset.X + (translateX / (100 * scaleTransform3D.ScaleX)) * 400;
                translateTransform3D.OffsetZ = diffOffset.Y + (translateY / (100 * scaleTransform3D.ScaleX)) * 400;
            }

            if (viewport1.IsMouseCaptured && e.MiddleButton == MouseButtonState.Pressed)
            {
                System.Windows.Point end = e.GetPosition(this);
                double angleX = end.X - start.X;
                double angleZ = end.Y - start.Y;
                double w = this.Width;
                double h = this.Height;
                double rotateX = (angleX * 100) / w;
                double rotateZ = (angleZ * 100) / h;

                axisAngleRotation3D.Angle += (rotateX + rotateZ);
                start = end;
            }
        }
        private void viewport1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            viewport1.CaptureMouse();
            start = e.GetPosition(this);
            diffOffset.X = translateTransform3D.OffsetX;
            diffOffset.Y = translateTransform3D.OffsetZ;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                mouseposition = e.GetPosition(viewport1);
                Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
                Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);

                PointHitTestParameters pointparams = new PointHitTestParameters(mouseposition);
                RayHitTestParameters rayparams = new RayHitTestParameters(testpoint3D, testdirection);

                VisualTreeHelper.HitTest(viewport1, null, HTResult, pointparams);
            }
        }
        private HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)
        {
            RayHitTestResult rayResult = rawresult as RayHitTestResult;

            if (rayResult != null)
            {
                bool gasit = false;
                tooltip.IsOpen = false;

                for (int i = 0; i < geometryModels.Count; i++)
                {
                    if (geometryModels.ElementAt(i).Value == rayResult.ModelHit)
                    {
                        long id = geometryModels.ElementAt(i).Key;
                        PowerEntity entity = entities.FirstOrDefault(e => e.Id == id);

                        string text = "ID: " + entity.Id + "\n" + "Name: " + entity.Name + "\n";

                        if (entity is SubstationEntity)
                        {
                            text += "Type: SubstationEntity";
                        }
                        else if (entity is NodeEntity)
                        {
                            text += "Type: NodeEntity";
                        }
                        else if (entity is SwitchEntity)
                        {
                            text += "Type: SwitchEntity";
                        }

                        tooltip.StaysOpen = false;
                        tooltip.IsOpen = true;
                        tooltip.Content = text;
                    }
                }

                for (int i = 0; i < geometryLines.Count; i++)
                {
                    if (geometryLines.ElementAt(i).Value.Any(e => e == rayResult.ModelHit))
                    {
                        long lineId = geometryLines.ElementAt(i).Key;
                        LineEntity line = lines.FirstOrDefault(e => e.Id == lineId);
                        PowerEntity firstEnd = entities.FirstOrDefault(e => e.Id == line.FirstEnd);
                        PowerEntity secondEnd = entities.FirstOrDefault(e => e.Id == line.SecondEnd);

                        GeometryModel3D newHitFirstEnd = geometryModels[firstEnd.Id];
                        GeometryModel3D newHitSecondEnd = geometryModels[secondEnd.Id];

                        Material newMaterial1 = geometryModels[firstEnd.Id].Material;
                        Material newMaterial2 = geometryModels[secondEnd.Id].Material;

                        DiffuseMaterial newMaterial = new DiffuseMaterial(new SolidColorBrush(System.Windows.Media.Colors.Purple));

                        if (hitGeometry1 != null)
                        {
                            hitGeometry1.Material = hitMaterial1;
                        }

                        if (hitGeometry2 != null)
                        {
                            hitGeometry2.Material = hitMaterial2;
                        }

                        gasit = true;

                        hitMaterial1 = newHitFirstEnd.Material;
                        hitMaterial2 = newHitSecondEnd.Material;

                        newHitFirstEnd.Material = newMaterial;
                        newHitSecondEnd.Material = newMaterial;

                        hitGeometry1 = newHitFirstEnd;
                        hitGeometry2 = newHitSecondEnd;

                    }
                }

                if (!gasit)
                {
                    if (hitGeometry1 != null)
                    {
                        hitGeometry1.Material = hitMaterial1;
                    }

                    if (hitGeometry2 != null)
                    {
                        hitGeometry2.Material = hitMaterial1;
                    }

                    hitGeometry1 = null;
                    hitGeometry2 = null;
                }
            }

            return HitTestResultBehavior.Stop;
        }

        #endregion MouseAction

        #region CheckBoxAction

        private void hideInactiveNetwork_Click(object sender, RoutedEventArgs e)
        {
            if (hideInactiveNetwork.IsChecked == true)
            {
                foreach (SwitchEntity s in switches.Where(o => o.Status == "Open"))
                {
                    if (geometryModels.ContainsKey(s.Id))
                    {
                        foreach (LineEntity l in lines.Where(o => o.FirstEnd == s.Id))
                        {
                            if (geometryModels.ContainsKey(l.SecondEnd))
                                myModel3DGroup.Children.Remove(geometryModels[l.SecondEnd]);

                            myModel3DGroup.Children.Remove(geometryModels[l.FirstEnd]);

                            if (geometryLines.ContainsKey(l.Id))
                            {
                                foreach (GeometryModel3D lp in geometryLines[l.Id])
                                {
                                    myModel3DGroup.Children.Remove(lp);
                                }
                            }
                        }
                    }
                }
            }
            else 
            {
                foreach (SwitchEntity s in switches.Where(o => o.Status == "Open"))
                {
                    if (geometryModels.ContainsKey(s.Id))
                    {
                        foreach (LineEntity l in lines.Where(o => o.FirstEnd == s.Id))
                        {
                            if (geometryModels.ContainsKey(l.SecondEnd))
                                myModel3DGroup.Children.Add(geometryModels[l.SecondEnd]);

                            myModel3DGroup.Children.Add(geometryModels[l.FirstEnd]);

                            if (geometryLines.ContainsKey(l.Id))
                            {
                                foreach (GeometryModel3D lp in geometryLines[l.Id])
                                {
                                    myModel3DGroup.Children.Add(lp);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void changeSwitchColor_Click(object sender, RoutedEventArgs e)
        {
            if (changeSwitchColor.IsChecked == true)
            {
                foreach (SwitchEntity s in switches)
                {
                    if (!geometryModels.ContainsKey(s.Id))
                        continue;

                    GeometryModel3D model = geometryModels[s.Id];

                    if (s.Status == "Open")
                    {
                        model.Material = new DiffuseMaterial(Brushes.Green);
                    }
                    else
                    {
                        model.Material = new DiffuseMaterial(Brushes.Red);

                    }
                }
            }
            else
            {
                foreach (SwitchEntity s in switches)
                {
                    if (!geometryModels.ContainsKey(s.Id))
                        continue;

                    GeometryModel3D model = geometryModels[s.Id];

                    model.Material = new DiffuseMaterial(Brushes.Orange);
                }
            }
        }

        private void changeLineColor_Click(object sender, RoutedEventArgs e)
        {
            if (changeLineColor.IsChecked == true)
            {
                foreach (LineEntity l in lines)
                {
                    if (!geometryLines.ContainsKey(l.Id))
                        continue;

                    foreach (GeometryModel3D lp in geometryLines[l.Id])
                    {
                        if (l.R < 1)
                        {
                            lp.Material = new DiffuseMaterial(Brushes.Red);
                        }
                        else if (l.R < 2)
                        {
                            lp.Material = new DiffuseMaterial(Brushes.Orange);
                        }
                        else
                        {
                            lp.Material = new DiffuseMaterial(Brushes.LightYellow);
                        }
                    }
                }
            }
            else
            {
                foreach (LineEntity l in lines)
                {
                    if (!geometryLines.ContainsKey(l.Id))
                        continue;

                    foreach (GeometryModel3D lp in geometryLines[l.Id])
                    {
                        if (l.ConductorMaterial == "Steel")
                            lp.Material = new DiffuseMaterial(Brushes.Yellow);
                        else if (l.ConductorMaterial == "Other")
                            lp.Material = lp.Material = new DiffuseMaterial(Brushes.Pink);
                        else if (l.ConductorMaterial == "Acsr")
                            lp.Material = lp.Material = new DiffuseMaterial(Brushes.Black);
                        else if (l.ConductorMaterial == "Copper")
                            lp.Material = lp.Material = new DiffuseMaterial(Brushes.Brown);
                    }
                }
            }
        }

        private void hideAllLine_Click(object sender, RoutedEventArgs e)
        {
            if (hideAllLine.IsChecked == true)
            {
                foreach (LineEntity l in lines)
                {
                    if (!geometryLines.ContainsKey(l.Id))
                        continue;

                    foreach (GeometryModel3D lp in geometryLines[l.Id])
                    {
                        myModel3DGroup.Children.Remove(lp);
                    }
                }
            }
            else
            {
                foreach (LineEntity l in lines)
                {
                    if (!geometryLines.ContainsKey(l.Id))
                        continue;

                    foreach (GeometryModel3D lp in geometryLines[l.Id])
                    {
                        myModel3DGroup.Children.Add(lp);
                    }
                }
            }
        }

        #endregion CheckBoxAction

    }
}
