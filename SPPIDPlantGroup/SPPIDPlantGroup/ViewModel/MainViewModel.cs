using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPPIDPlantGroup
{
    public class MainViewModel
    {
        internal ObservableCollection<PlantGroup> plantGroups = 
            new ObservableCollection<PlantGroup>();

        SqlConnection conn = null;
        SqlDataReader oDR = null;

        internal PlantGroup oRootItem = null;
        internal List<PlantGroup> rootSystem = null;
        List<PlantGroup> children = new List<PlantGroup>();

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
                    pg.IsExpanded = false;

                    plantGroups.Add(pg);
                }

                // Set the rootitem
                this.GetPlantRootItem();

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
            if (this.plantGroups.Count > 0)
            {
                oRootItem = plantGroups.Where(x => x.ParentID == "-1").FirstOrDefault();
                oRootItem.IsExpanded = true;

                if(oRootItem != null)
                {
                    rootSystem = plantGroups.Where(x => x.ParentID == oRootItem.SPID).ToList();
                }
            }
            else
                oRootItem = null;
        }

        internal void FindChildren(string itemSPID)
        {
            if(this.plantGroups.Count > 0)
            {
                this.children.Clear();

                children = plantGroups.Where(x => x.ParentID == itemSPID).ToList();
            }
        }
    }
}
