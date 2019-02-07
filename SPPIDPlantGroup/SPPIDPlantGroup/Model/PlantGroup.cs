using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPPIDPlantGroup
{
    public class PlantGroup
    {
         #region Initializers and Properties

        private PlantGroup _parent;
        private bool _isExpanded = false;
        private bool _isSelected = false;
        
        private List<PlantGroup> _children =
            new List<PlantGroup>();

        private Dictionary<int, PlantGroupHierarchy> _hierarchyTree =
            new Dictionary<int, PlantGroupHierarchy>();


        [DisplayName("SPPID ID")]
        [Bindable(true, BindingDirection.TwoWay)]
        public string SPID { get; set; }

        [DisplayName("Unit/Area Name")]
        [Bindable(true, BindingDirection.TwoWay)]
        public string Name { get; set; }

        [DisplayName("Unit/Area Description")]
        [Bindable(true, BindingDirection.TwoWay)]
        public string Description { get; set; }

        [DisplayName("Full Path")]
        [Bindable(true, BindingDirection.TwoWay)]
        public string Path { get; set; }

        [DisplayName("Parent SPID")]
        [Bindable(true, BindingDirection.TwoWay)]
        public string ParentID { get; set; }

        [DisplayName("System Type")]
        [Bindable(true, BindingDirection.TwoWay)]
        public PlantGroupType GroupType { get; set; }
        
        [DisplayName("Children")]
        [Bindable(true, BindingDirection.TwoWay)]
        public List<PlantGroup> Children
        {
            get { return _children; }
            set { _children = value; }
        }

        [DisplayName("Tree")]
        [Bindable(true, BindingDirection.TwoWay)]
        public  Dictionary<int, PlantGroupHierarchy> HierarchyTree
        {
            get { return _hierarchyTree; }
            set 
            { 
                _hierarchyTree = value;
                PropertyHadChangedValues("HierarchyTree");
            }
        }

        public PlantGroup Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public object Image { get; set; }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                PropertyHadChangedValues("IsExpanded");
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                PropertyHadChangedValues("IsSelected");
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PlantGroup()
        { }

        /// <summary>
        ///  Constructor based on Parent Group
        /// </summary>
        /// <param name="oParent"></param>
        public PlantGroup(PlantGroup oParent)
        {
            this.Parent = oParent;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the Plant Group Type
        /// </summary>
        /// <returns></returns>
        public string GetPlantGroupType()
        {
            return this.GroupType.ToString();
        }


        /// <summary>
        /// IComparable Implementations
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            PlantGroup a = this;
            PlantGroup b = (PlantGroup)obj;
            return (a.Name.CompareTo(b.Name));
        }

        /// <summary>
        /// Set the Image for this System
        /// </summary>
        internal void SetImage()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

            try
            {
                switch (this.GroupType)
                {
                    case PlantGroupType.MainProject:
                        {
                            image.UriSource = new Uri(@"pack://application:,,,/Resources/SPPID/SiteRoot.ico");
                            break;
                        }
                    case PlantGroupType.Plant:
                        {
                            image.UriSource = new Uri(@"pack://application:,,,/Resources/SPPID/PlantRoot.ico");
                            break;
                        }
                    case PlantGroupType.Area:
                        {
                            image.UriSource = new Uri(@"pack://application:,,,/Resources/SPPID/Unit.ico");
                            break;
                        }

                    case PlantGroupType.Unit:
                        {
                            image.UriSource = new Uri(@"pack://application:,,,/Resources/SPPID/Area.ico");
                            break;
                        }
                    default:
                        {
                            image.UriSource = new Uri(@"pack://application:,,,/Resources/SPPID/icoHelp.ico");
                            break;
                        }
                }
            }
            catch 
            {
                image.UriSource = new Uri(@"pack://application:,,,/Resources/SPPID/icoHelp.ico");
            }
            finally
            {
                image.EndInit();
                this.Image = image;
            }
        }

        #endregion
    }
}
