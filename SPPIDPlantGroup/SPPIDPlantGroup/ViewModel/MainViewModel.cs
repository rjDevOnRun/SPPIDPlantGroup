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
        private ObservableCollection<PipeSystem> _pipingSystems =
            new ObservableCollection<PipeSystem>();

        private ObservableCollection<PipeLine> _sppidPipeLines =
            new ObservableCollection<PipeLine>();

        private ObservableCollection<PIDDrawing> _sppidDrawings =
            new ObservableCollection<PIDDrawing>();

        private ObservableCollection<PlantGroup> _sppidSystems =
            new ObservableCollection<PlantGroup>();

        private Dictionary<string, string> _dicPipeProps =
            new Dictionary<string, string>();

        private ObservableCollection<PlantGroup> _plantRootSys =
            new ObservableCollection<PlantGroup>();

        /// <summary>
        /// PipeLine Counts
        /// </summary>
        public string SPPIDLineCount
        {
            get
            {
                return "Total PipeSystems: " +
                        this._pipingSystems.Count.ToString() +
                        " nos.";
            }
        }

        /// <summary>
        /// The View object tied to this View-Model...
        /// </summary>
        public ARSView oArsUI { get; set; }

        /// <summary>
        /// Current Selected Plant Group Item
        /// </summary>
        public PlantGroup CurrentSelectedPlantGroup { get; set; }

        /// <summary>
        /// Current Selected PID Drawing
        /// </summary>
        public PIDDrawing SelectedPIDDrawing { get; set; }

        /// <summary>
        /// Current Selected Pipe System
        /// </summary>
        public PipeSystem SelectedPipeSystem { get; set; }

        /// <summary>
        /// Collection of Piping Systems in SPPID Database
        /// </summary>
        public ObservableCollection<PipeSystem> PipingSystems
        {
            get { return _pipingSystems; }
            set 
            { 
                _pipingSystems = value;
                PropertyHadChangedValues("PipingSystems");
            }
        }

        /// <summary>
        /// Collection of PipeLines in SPPID
        /// </summary>
        public ObservableCollection<PipeLine> SPPIDPipeLines
        {
            get { return _sppidPipeLines; }
            set 
            { 
                _sppidPipeLines = value;
                PropertyHadChangedValues("SPPIDPipeLines");
            }
        }

        /// <summary>
        /// Collection of PID Drawings in SPPID
        /// </summary>
        public ObservableCollection<PIDDrawing> SPPIDDrawings
        {
            get { return _sppidDrawings; }
            set { _sppidDrawings = value; }
        }

        /// <summary>
        /// Collection of SPPID Systems (Plant/Area/Unit)
        /// </summary>
        public ObservableCollection<PlantGroup> SPPIDSystems
        {
            get { return _sppidSystems; }
            set 
            { 
                _sppidSystems = value;
                PropertyHadChangedValues("SPPIDSystems");
            }
        }

        /// <summary>
        /// Main Plant Root System
        /// </summary>
        public ObservableCollection<PlantGroup> PlantRootSystem
        {
            get { return _plantRootSys; }
            set 
            { 
                _plantRootSys = value;
                PropertyHadChangedValues("PlantRootSystem");
            }
        }

        /// <summary>
        /// Dictionary storage of Pipeline Properties
        /// </summary>
        public Dictionary<string, string> PipeProps
        {
            get { return _dicPipeProps; }
            set 
            { 
                _dicPipeProps = value;
                PropertyHadChangedValues("PipeLineProperty");
            }
        }

        #endregion

        #region Constructors

        public ARSViewModel(ARSView oView)
        {
            this.oArsUI = oView;

            // Initialize ARS Settings
            if (!InitializeARSApplication())
            {
                MessageBox.Show("Error Happened!");
                return;
            }
        }

        #endregion

        #region Commands

        ICommand _loadDrawings;
        ICommand _loadPipeSystems;
        ICommand _generateFromTo;
        ICommand _populateARSDatabase;

        /// <summary>
        /// PublishDataToARSdB Command Predicate
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanPopulateARSDatabase(object obj)
        {
            try
            {
                if (this.PipingSystems.Count == 0)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Command for Publishing Data into ARS DB
        /// </summary>
        public ICommand PopulateARSDatabaseCmd
        {
            get
            {
                if(_populateARSDatabase == null)
                {
                    _populateARSDatabase = new DelegateCommand<object>(WriteDataToARSCentralDatabases, CanPopulateARSDatabase);
                }
                return _populateARSDatabase;
            }
        }

        /// <summary>
        /// Command for GenerateFromTo data
        /// </summary>
        public ICommand GenerateFromToCmd
        {
            get
            {
                if (_generateFromTo == null) 
                {
                    _generateFromTo = new DelegateCommand<object>(GenerateFromToDataOfPipingSystems, CanGenerateFromTo);
                }
                return _generateFromTo;
            }           
        }

        /// <summary>
        /// GenerateFromTo Command Predicate
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanGenerateFromTo(object obj)
        {
            try
            {
                if (this.PipingSystems.Count == 0)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Command Bound to PlantGroup ListBox
        /// </summary>
        public ICommand LoadDrawingsCmd
        {
            get
            {
                if (_loadDrawings == null)
                {
                    _loadDrawings = new DelegateCommand<object>(LoadDrawingsInThisGroup, CanLoadDrawings);
                }
                
                return _loadDrawings;
            }
        }

        /// <summary>
        /// Command bound to Drawings ListBox
        /// </summary>
        public ICommand LoadPipeSystemsCmd
        {
            get
            {
                if (_loadPipeSystems == null)
                {
                    _loadPipeSystems = new DelegateCommand<object>(LoadPipingSystemsInThisDrawing, CanLoadPipeSystems);
                }
                return _loadPipeSystems;
            }
        }
       
        /// <summary>
        /// Load-Drawings Command Predicate Method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanLoadDrawings(object obj)
        {
            try
            {
                if (this.CurrentSelectedPlantGroup == null)
                    return false;
                else
                {
                    this.LoadAllPipingSystemsInThisGroup(this.CurrentSelectedPlantGroup);
                    return true;
                }   
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Load-PipeSystems Command Predicate
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool CanLoadPipeSystems(object obj)
        {
            try
            {
                if (this.SelectedPIDDrawing == null)
                {
                    return false;
                }
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load all the Plant Systems of SPPID project
        /// </summary>
        public void LoadPlantSystems()
        {
            using (new WaitCursor())
            {
                try
                {
                    // #1: Collect all Systems from Plant DB
                    string queryPlantGroup = QueryStore.CollectPlantGroups();

                    DbDataReader results = (DbDataReader)DBUtils.GetResultAsDataReaderFromDB(queryPlantGroup);

                    if (results != null)
                    {
                        CreatePlantGroupCollection(results);
                    }   
                    else
                        this._sppidSystems.Clear();

                    // #2: Create the Root System and display in Treeview
                    this.CreateRootSystem();

                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message + ex.InnerException + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Load all the PID Drawings of the selected Plant System
        /// </summary>
        /// <param name="obj"></param>
        public void LoadDrawingsInThisGroup(object obj)
        {
            this.PipingSystems.Clear();

            PlantGroup pgItem = (PlantGroup)obj;

            using (new WaitCursor())
            {
                try
                {
                    DbDataReader results = (DbDataReader)DBUtils.GetResultAsDataReaderFromDB
                                            (QueryStore.CollectPIDDrawings(pgItem.SPID.ToString()));

                    if (results != null && results.HasRows)
                    {
                        this.SPPIDPipeLines.Clear();
                        CreateDrawingsCollection(results);
                    }
                    else
                        this._sppidDrawings.Clear();
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message + ex.InnerException + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Load all the Pipe Systems of the selected Group
        /// </summary>
        /// <param name="pgItem"></param>
        public void LoadAllPipingSystemsInThisGroup(PlantGroup pgItem)
        {
            using (new WaitCursor())
            {
                try
                {
                    this._pipingSystems.Clear();
                    
                    // Collect Query for Pipe Systems
                    DbDataReader results = (DbDataReader)DBUtils.GetResultAsDataReaderFromDB(
                                                            QueryStore.CollectPipingSystemsInPlantGroup(pgItem.SPID.ToString()));

                    if (results != null)
                    {
                        CreatePipingSystemCollection(results);
                        //CreateSPPIDLinesCollection(results);
                        this.SPPIDPipeLines.Sort();
                    }
                    else
                        this.SPPIDPipeLines.Clear();

                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message + ex.InnerException + ex.StackTrace);
                }

                PropertyHadChangedValues("SPPIDLineCount");
                Helpers.ProcessAllWindowsMessagesInMsgQueue();
            }
        }

        /// <summary>
        /// Load all the Piping Systems in the selected PID Drawing
        /// </summary>
        /// <param name="obj"></param>
        public void LoadPipingSystemsInThisDrawing(object obj)
        {
            PIDDrawing drawing = (PIDDrawing)obj;

            using (new WaitCursor())
            {
                try
                {
                    this._pipingSystems.Clear();

                    // Collect Query for Pipe Systems
                    DbDataReader results = (DbDataReader)DBUtils.GetResultAsDataReaderFromDB(
                                                            QueryStore.CollectPipingSystemsInDrawing(
                                                                        drawing.PlantGroupID.ToString(),
                                                                        drawing.SPID.ToString()));

                    if (results != null)
                    {
                        CreatePipingSystemCollection(results);
                        //CreateSPPIDLinesCollection(results);
                        this.SPPIDPipeLines.Sort();
                    }
                    else
                        this.SPPIDPipeLines.Clear();

                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message + ex.InnerException + ex.StackTrace);
                }

                PropertyHadChangedValues("SPPIDLineCount");
                Helpers.ProcessAllWindowsMessagesInMsgQueue();
            }
        }

        /// <summary>
        /// Generate the FROM-TO data
        /// </summary>
        public void GenerateFromToData()
        {
          
            using (new WaitCursor())
            {
                for (int i = 0; i < this.SPPIDPipeLines.Count - 1; i++)
                {
                    PipeLine pl = this.SPPIDPipeLines[i];

                    FromToCalculator ftCal = new FromToCalculator(ref pl);

                    var bSucess = ftCal.InitializeFromToCalculation();

                    if (ftCal.FromData.Count > 0)
                    {
                        foreach (string str in ftCal.FromData)
                            pl.From += str;
                    }

                    if (ftCal.ToData.Count > 0)
                    {
                        foreach (string str in ftCal.ToData)
                            pl.To += str;
                    }

                    ftCal = null;
                }
            }
        }

        /// <summary>
        /// (Command Method) Generate From-To informations
        /// </summary>
        /// <param name="obj"></param>
        public void GenerateFromToDataOfPipingSystems(object obj)
        {

            using (new WaitCursor())
            {
                for (int i = 0; i < this._pipingSystems.Count - 1; i++)
                {
                    PipeSystem ps = this._pipingSystems[i];
                    PipeLine pl = new PipeLine();
                    pl.PipeSystem = ps.Name;

                    FromToCalculator ftCal = new FromToCalculator(ref ps);

                    var bSucess = ftCal.InitializeFromToCalculation();

                    if (ftCal.FromData.Count > 0)
                    {
                        foreach (string str in ftCal.FromData)
                        {
                            pl.From += str;
                            ps.From += str;

                        }
                    }

                    if (ftCal.ToData.Count > 0)
                    {
                        foreach (string str in ftCal.ToData)
                        {
                            pl.To += str;
                            ps.To += str;
                        }   
                    }

                    ftCal = null;
                    pl = null;
                }
            }
            // Refresh the Datagrid to display the calculated information
            oArsUI.dgLineData.Items.Refresh();
        }

        /// <summary>
        /// Publish the Calculated data into ARS Central Databases
        /// </summary>
        /// <param name="obj"></param>
        public void WriteDataToARSCentralDatabases(object obj)
        {
            SQLiteUtils sqlt = new SQLiteUtils();

            try
            {
                if (!sqlt.ConnectToSQLiteDB()) return;

                using (new WaitCursor())
                {
                    // Write the From-To Data DB
                    // We need to cleanup existing record in Table first
                    string sqlCleaup = "DELETE FROM SPPIDFromTo";
                    sqlt.ExecuteQuery(sqlCleaup);


                    string fData = string.Empty;
                    string tData = string.Empty;

                    foreach (PipeSystem ps in this.PipingSystems)
                    {
                        if (string.IsNullOrEmpty(ps.From))
                            fData = string.Empty;
                        else
                            fData = ps.From.ToString();

                        if (string.IsNullOrEmpty(ps.To))
                            tData = string.Empty;
                        else
                            tData = ps.To.ToString();

                        string queryInsert = "insert into SPPIDFromTo (PipeSystem, FromItem, ToItem) values ('" + ps.Name.ToString() + "', '" + fData + "', '" + tData + "')";
                        sqlt.ExecuteQuery(queryInsert);
                    }
                }
            }
            finally
            {
                // Dont forget to close the DB connection
                sqlt.CloseDBConnection();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create the Pipe System collection
        /// </summary>
        /// <param name="results"></param>
        private void CreatePipingSystemCollection(DbDataReader results)
        {
            this.PipingSystems.Clear();

            while (results.Read())
            {
                // Collect the result into a PipeLine object
                // and pass it into PipeSystem object.
                PipeLine pl = new PipeLine();
                try
                {
                    if (results.IsDBNull(0))
                        pl.FluidCode = string.Empty;
                    else
                        pl.FluidCode = results[0].ToString();

                    if (results.IsDBNull(1))
                        pl.LineSeqNo = string.Empty;
                    else
                        pl.LineSeqNo = results[1].ToString();

                    if (results.IsDBNull(2))
                        pl.Unit = string.Empty;
                    else
                        pl.Unit = results[2].ToString();

                    if (results.IsDBNull(3))
                        pl.PipingMatlClass = string.Empty;
                    else
                        pl.PipingMatlClass = results[3].ToString();

                    if (results.IsDBNull(4))
                        pl.InsulationCode = string.Empty;
                    else
                        pl.InsulationCode = results[4].ToString();

                    PipeSystem ps = new PipeSystem(pl);
                    this._pipingSystems.Add(ps);

                    pl = null;

                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message + ex.InnerException + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Build SPPID Lines Collection
        /// </summary>
        /// <param name="results"></param>
        private void CreateSPPIDLinesCollection(DbDataReader results)
        {
            this.SPPIDPipeLines.Clear();

            while(results.Read())
            {
                PipeLine pl = new PipeLine();
                try
                {
                    //pl.SPID = results[0].ToString();
                    pl.PipeSize = results[0].ToString();
                    pl.FluidCode = results[1].ToString();
                    pl.LineSeqNo = results[2].ToString();
                    pl.Unit = results[3].ToString();
                    pl.PipingMatlClass = results[4].ToString();
                    pl.InsulationCode = results[5].ToString();
                    pl.PipeItemTag = results[6].ToString();
                    pl.SteamOut = results[7].ToString();
                    pl.SteamOutTemp = results[8].ToString();
                    pl.HeatTraceMed = results[9].ToString();

                    // Mapped LineID
                    pl.CreateMappedLineID();

                    this._sppidPipeLines.Add(pl);

                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message + ex.InnerException + ex.StackTrace);
                }
            }

            // Query already has Orderby clause.
            //this._sppidPipeLines.Sort();

        }

        /// <summary>
        /// Build Drawings Collection
        /// </summary>
        /// <param name="results"></param>
        private void CreateDrawingsCollection(DbDataReader results)
        {
            this.SPPIDDrawings.Clear();
            
            while(results.Read())
            {
                PIDDrawing dwg = new PIDDrawing();

                try
                {
                    dwg.SPID = results[0].ToString();
                    dwg.Name = results[1].ToString();
                    dwg.Number = results[2].ToString();
                    dwg.Title = results[3].ToString();
                    dwg.FullPath = results[4].ToString();

                    if (results.IsDBNull(5))
                    {
                        dwg.ItemStatus = ItemStatus.Delete_Pending;
                    }
                    else
                    {
                        switch(results[5].ToString())
                        {
                            case "1":
                                {
                                    dwg.ItemStatus = ItemStatus.Active;
                                    break;
                                }
                            case "2":
                            case "3":
                            case "4":
                            case "5":
                                {
                                    // It means these are not available in DB
                                    dwg.ItemStatus = ItemStatus.Delete_Pending;
                                    break;
                                }
                            case "6":
                                {
                                    dwg.ItemStatus = ItemStatus.Recreate_Pending;
                                    break;
                                }
                            default:
                                {
                                    dwg.ItemStatus = ItemStatus.Delete_Pending;
                                    break;
                                }
                        }
                    }

                    dwg.PlantGroupID = results[6].ToString();

                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message + ex.InnerException + ex.StackTrace);
                }

                this._sppidDrawings.Add(dwg);
            }

            if (this._sppidDrawings.Count > 0)
                this._sppidDrawings.Sort();
        }

        /// <summary>
        /// Build Plant Group Items Collection
        /// </summary>
        /// <param name="dbResult"></param>
        private void CreatePlantGroupCollection(DbDataReader dbResult)
        {
            while(dbResult.Read())
            {
                PlantGroup pgItem = new PlantGroup();

                try
                {
                    pgItem.SPID = dbResult[0].ToString();
                    pgItem.Name = dbResult[1].ToString();
                    pgItem.Description = dbResult[2].ToString();
                    pgItem.Path = dbResult[3].ToString();
                    pgItem.ParentID = dbResult[4].ToString();

                    if (dbResult.IsDBNull(5))
                    {
                        pgItem.GroupType = PlantGroupType.UnKnown;
                    }
                    else
                    {
                        switch (dbResult[5].ToString())
                        {
                            case "70":
                                {
                                    pgItem.GroupType = PlantGroupType.Plant;
                                    break;
                                }
                            case "72":
                                {
                                    pgItem.GroupType = PlantGroupType.Unit;
                                    break;
                                }
                            case "65":
                                {
                                    pgItem.GroupType = PlantGroupType.Area;
                                    break;
                                }
                            case "69":
                                {
                                    pgItem.GroupType = PlantGroupType.MainProject;
                                    break;
                                }
                            default:
                                {
                                    pgItem.GroupType = PlantGroupType.UnKnown;
                                    break;
                                }
                        }
                    }

                    pgItem.SetImage();

                    this._sppidSystems.Add(pgItem);
                    //this._plantRootSys.Add(pgItem);

                    // Set the Root Plant System
                    if (pgItem.ParentID == "-1")
                        this.CurrentSelectedPlantGroup = pgItem;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message + ex.InnerException + ex.StackTrace);
                }
            }

            if(this._sppidSystems.Count>0)
                this._sppidSystems.Sort();

        }

        /// <summary>
        /// Generate the Main Plant-Root-System
        /// </summary>
        private void CreateRootSystem()
        {
            // Top-Root System is the one which does not have a parent("-1")
            PlantGroup pgRoot = this._sppidSystems.Where(x => x.ParentID == "-1").FirstOrDefault();

            if(pgRoot != null)
            {
                try
                {

                    if (this._plantRootSys.Count > 0)
                        this._plantRootSys.Clear();

                    // Get all the Child Systems of the Top-Roo Sysytem
                    List<PlantGroup> child = this._sppidSystems.Where(x => x.ParentID == pgRoot.SPID).ToList();
                    pgRoot.Children.AddRange(child);

                    // Add this to the Display collection of the Treeview
                    this._plantRootSys.Add(pgRoot);

                    // Make this the current selected System
                    this.CurrentSelectedPlantGroup = pgRoot;

                }
                catch(Exception ex)
                {
                    Debug.Print(ex.Message + ex.InnerException + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Event handler for TreeView Selection Change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void SelectedItemHandleInPlantTreeView(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                // Parse into a plantgroup object
                PlantGroup pgItem = (sender as TreeView).SelectedItem as PlantGroup;

                // return if its is null
                if (pgItem == null) return;

                // Make it expanded
                pgItem.IsExpanded = true;
                
                // Make it the current selected system
                this.CurrentSelectedPlantGroup = pgItem;

                // Load all the PID drawings of this system (if any)
                LoadDrawingsInThisGroup(this.CurrentSelectedPlantGroup);

                // return if its child is not-empty
                // it means that we have already generated the children
                if (pgItem.Children.Count > 0) return;

                // Get all the Child systems of the selected system
                List<PlantGroup> child = this._sppidSystems.Where(x => x.ParentID == pgItem.SPID).ToList();

                // if found Children
                if(child.Count > 0)
                {
                    try
                    {
                        // add them to Children of the selected system
                        pgItem.Children.AddRange(child);

                        // temp store the current root system
                        var rootSystems = this._plantRootSys[0];
                        this._plantRootSys.Clear(); // clear it
                        
                        // Re-Order all the system which includes 
                        // the newly found children
                        SortData(rootSystems);

                        // Set the main plant-root-systems for display
                        this._plantRootSys.Add(rootSystems);
                    }
                    catch(Exception ee)
                    {
                        Debug.Print(ee.Message + ee.InnerException + ee.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignore errors at this moment
                Debug.Print(ex.Message + ex.InnerException + ex.StackTrace);
            }
        }

        /// <summary>
        /// Recursively Re-Order each Plant System based on their Parents
        /// </summary>
        /// <param name="oObj"></param>
        private void SortData(PlantGroup oObj)
        {
            if (oObj.Children.Count > 0)
            {
                oObj.Children = oObj.Children.OrderBy(x => x.ParentID.ToString()).ToList();

                foreach (var oChild in oObj.Children)
                    SortData(oChild);
            }
        }


        /// <summary>
        /// Initialize the ARS PreRequisites
        /// </summary>
        /// <returns></returns>
        private bool InitializeARSApplication()
        {
            using (new WaitCursor())
            {
                // TODO: Allow writing back to XML file...
                // Retrieve Application Settings
                Helpers.CollectApplicationSettings();

                // Store SPPID ItemTag format
                Globals.PipeRunItemTagFormat =
                    Helpers.CollectSPPIDItemTagFormat();

                // Initialize the SPPID Items Priority Object
                Helpers.InitializePriorityList();

                // Collect PipeRun Properties
                Helpers.GetPipeDescriptionAttributes(ref this._dicPipeProps);

                // Establish Connection to SPPID Database
                return DBUtils.ConnectToDB();
            }
        }

        #endregion
}
