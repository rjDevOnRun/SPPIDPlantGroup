using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPPIDPlantGroup
{
    public class MainViewModel: INotifyPropertyChanged
    {
        internal ObservableCollection<PlantGroup> plantGroups = 
            new ObservableCollection<PlantGroup>();

        SqlConnection conn = null;
        SqlDataReader oDR = null;

        internal PlantGroup oRootItem = null;
        internal List<PlantGroup> rootSystem = null;
        //List<PlantGroup> children = new List<PlantGroup>();

        private List<PlantGroup> _children = 
            new List<PlantGroup>();

        public List<PlantGroup> Children
        {
            get { return _children; }
            set
            {
                _children = value;
                NotifyPropertyHasChanged("Children");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyHasChanged(string propertyname)
        {
            if(this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public MainViewModel()
        {

        }

        internal void ConnectToDB()
        {
            string connectionString = "Server=DELL_PC;Database=SPPID;User Id=sa;Password = Oracle01; ";
            string sqlPG = "select pg.SP_ID, pg.ParentID, pg.Name, pg.Description, pg.PlantGroupType, pg.Dir_Path " +
                            "from SPPIDTestPlant.T_PlantGroup pg;";

            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();

                SqlCommand command = new SqlCommand(sqlPG, conn);
                oDR = command.ExecuteReader();

                while (oDR.Read())
                {
                    PlantGroup pg = new PlantGroup();

                    pg.SPID = oDR[0].ToString();
                    pg.ParentID = oDR[1].ToString();
                    pg.Name = oDR[2].ToString();
                    pg.Description = oDR[3].ToString();
                    pg.PlantGroupType = oDR[4].ToString();
                    pg.Path = oDR[5].ToString();
                    pg.IsExpanded = true;

                    plantGroups.Add(pg);
                }

                // Set the rootitem
                this.GetPlantRootItem();

                //// Find Childrens of all items
                //foreach (PlantGroup pg in this.plantGroups)
                //{
                //    FindChildren(pg);
                //}
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message + ex.InnerException);
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        private void GetPlantRootItem()
        {
            _children.Clear();

            if (this.plantGroups.Count > 0)
            {
                oRootItem = plantGroups.Where(x => x.ParentID == "-1").FirstOrDefault();
                oRootItem.IsExpanded = true;

                if(oRootItem != null)
                {
                    _children = plantGroups.Where(x => x.ParentID == oRootItem.SPID).ToList();
                }
            }
            else
                oRootItem = null;
        }

        internal void FindChildren(PlantGroup pgItem)
        {
            _children.Clear();

            if (this.plantGroups.Count > 0)
            {
                pgItem.Children = new List<PlantGroup>();

                _children = plantGroups.Where(x => x.ParentID == pgItem.SPID).ToList();

                pgItem.Children.AddRange(_children);
            }
            
        }

        internal void FindChildren(string itemSPID)
        {
            if(this.plantGroups.Count > 0)
            {
                this._children.Clear();

                _children = plantGroups.Where(x => x.ParentID == itemSPID).ToList();
            }
        }
    }
}
