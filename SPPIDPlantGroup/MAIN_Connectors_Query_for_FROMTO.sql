-- ======================================================================================================
-- From-to Main Query

select distinct
con.SP_ID as connector_spid,
pr.piperuntype as PipeRunType,
pr.SP_ID as PipeRunSPID,
pr.FlowDirection as FlowDirection,
(select rpp.RepresentationType from SPPIDTestPlantpid.T_Representation rpp where rpp.SP_ID=con.SP_ConnectItem1ID) as representationType,
rep.ItemStatus,
con.SP_ConnectItem1ID as ConnectItem_SPID,
mod.ItemTypeName,
(case	when mod.ItemTypeName = 'OPC' then (select opc.sp_pairedwithid from SPPIDTestPlantpid.T_OPC opc where opc.SP_ID=mod.SP_ID)
		when mod.ItemTypeName = 'Nozzle' then (select noz.sp_equipmentid from SPPIDTestPlantpid.T_Nozzle noz where noz.SP_ID=mod.SP_ID)
		when mod.ItemTypeName = 'PipingComp' then (select ilc.sp_piperunid from SPPIDTestPlantpid.T_InlineComp ilc where ilc.SP_PipingCompID=mod.SP_ID)
		when mod.ItemTypeName = 'Instrument' then (select ilc.sp_piperunid from SPPIDTestPlantpid.T_InlineComp ilc where ilc.SP_InstrumentID=mod.SP_ID)
		end
) as ConnectedItemSPID,
'End1' as Connector_onWhichSide,
(case	when mod.ItemTypeName = 'PipeRun' then (pr.NominalDiameter)
		when mod.ItemTypeName = 'PipingComp' then (select prp.NominalDiameter from SPPIDTestPlantpid.T_PipeRun prp where prp.SP_ID =rep.SP_ModelItemID)
		when mod.ItemTypeName = 'Nozzle' then (select noz.nominaldiameter from SPPIDTestPlantpid.T_Nozzle noz where noz.SP_ID=mod.SP_ID)
		end
) as NominalDiameter,
(case	when mod.ItemTypeName = 'PipeRun' then (pr.FlowDirection)
		when mod.ItemTypeName = 'PipingComp' then (select prp.FlowDirection from SPPIDTestPlantpid.T_PipeRun prp where prp.SP_ID =rep.SP_ModelItemID)
		when mod.ItemTypeName = 'Nozzle' then (select noz.FlowDirection from SPPIDTestPlantpid.T_Nozzle noz where noz.SP_ID=mod.SP_ID)
		end
) as ModelItemflowdirection,
(case	when mod.ItemTypeName = 'Nozzle' then (select plp.itemtag from SPPIDTestPlantpid.T_PlantItem plp
																	inner join SPPIDTestPlantpid.T_Nozzle nzz on nzz.SP_ID=plp.SP_ID
																	where plp.SP_ID=nzz.SP_EquipmentID)
		when mod.ItemTypeName = 'OPC' then (select opc.OPCTag from SPPIDTestPlantpid.T_OPC opc where opc.SP_ID=mod.SP_ID)
		else (select pll.itemtag from SPPIDTestPlantpid.T_PlantItem pll where pll.SP_ID=mod.SP_ID)
		end
) as ITEMTAG,
con.IsZeroLength

from SPPIDTestPlantpid.T_Connector con, SPPIDTestPlantpid.T_ModelItem mod, 
	 SPPIDTestPlantpid.T_Representation rep, SPPIDTestPlantpid.T_PipeRun pr
where rep.SP_ID=con.SP_ID
and con.SP_ID in (select rp.sp_id from SPPIDTestPlantpid.T_Representation rp 
						left outer join SPPIDTestPlantpid.T_ModelItem ml on ml.SP_ID=rp.SP_ModelItemID
						where ml.SP_ID in (select pll.sp_id from SPPIDTestPlantpid.T_PlantItem pll
												where pll.ItemTag like '%10"-105-DS-A1A%'))
AND mod.SP_ID = (select rpp.sp_modelitemid from SPPIDTestPlantpid.T_Representation rpp where rpp.SP_ID=con.SP_ConnectItem1ID)
and PR.SP_ID=rep.SP_ModelItemID


union all

select distinct
con.SP_ID as connector_spid,
pr.piperuntype as PipeRunType,
pr.SP_ID as PipeRunSPID,
pr.FlowDirection as FlowDirection,
(select rpp.RepresentationType from SPPIDTestPlantpid.T_Representation rpp where rpp.SP_ID=con.SP_ConnectItem1ID) as representationType,
rep.ItemStatus,
con.SP_ConnectItem2ID as ConnectItem_SPID,
mod.ItemTypeName,
(case	when mod.ItemTypeName = 'OPC' then (select opc.sp_pairedwithid from SPPIDTestPlantpid.T_OPC opc where opc.SP_ID=mod.SP_ID)
		when mod.ItemTypeName = 'Nozzle' then (select noz.sp_equipmentid from SPPIDTestPlantpid.T_Nozzle noz where noz.SP_ID=mod.SP_ID)
		when mod.ItemTypeName = 'PipingComp' then (select ilc.sp_piperunid from SPPIDTestPlantpid.T_InlineComp ilc where ilc.SP_PipingCompID=mod.SP_ID)
		when mod.ItemTypeName = 'Instrument' then (select ilc.sp_piperunid from SPPIDTestPlantpid.T_InlineComp ilc where ilc.SP_InstrumentID=mod.SP_ID)
		end
) as ConnectedItemSPID,
'End2' as Connector_onWhichSide,
(case	when mod.ItemTypeName = 'PipeRun' then (pr.NominalDiameter)
		when mod.ItemTypeName = 'PipingComp' then (select prp.NominalDiameter from SPPIDTestPlantpid.T_PipeRun prp where prp.SP_ID =rep.SP_ModelItemID)
		when mod.ItemTypeName = 'Nozzle' then (select noz.nominaldiameter from SPPIDTestPlantpid.T_Nozzle noz where noz.SP_ID=mod.SP_ID)
		end
) as NominalDiameter,
(case	when mod.ItemTypeName = 'PipeRun' then (pr.FlowDirection)
		when mod.ItemTypeName = 'PipingComp' then (select prp.FlowDirection from SPPIDTestPlantpid.T_PipeRun prp where prp.SP_ID =rep.SP_ModelItemID)
		when mod.ItemTypeName = 'Nozzle' then (select noz.FlowDirection from SPPIDTestPlantpid.T_Nozzle noz where noz.SP_ID=mod.SP_ID)
		end
) as ModelItemflowdirection,
(case	when mod.ItemTypeName = 'Nozzle' then (select plp.itemtag from SPPIDTestPlantpid.T_PlantItem plp
																	inner join SPPIDTestPlantpid.T_Nozzle nzz on nzz.SP_ID=plp.SP_ID
																	where plp.SP_ID=nzz.SP_EquipmentID)
		when mod.ItemTypeName = 'OPC' then (select opc.OPCTag from SPPIDTestPlantpid.T_OPC opc where opc.SP_ID=mod.SP_ID)
		else (select pll.itemtag from SPPIDTestPlantpid.T_PlantItem pll where pll.SP_ID=mod.SP_ID)
		end
) as ITEMTAG,
con.IsZeroLength

from SPPIDTestPlantpid.T_Connector con, SPPIDTestPlantpid.T_ModelItem mod, 
	 SPPIDTestPlantpid.T_Representation rep, SPPIDTestPlantpid.T_PipeRun pr
where rep.SP_ID=con.SP_ID
and con.SP_ID in (select rp.sp_id from SPPIDTestPlantpid.T_Representation rp 
						left outer join SPPIDTestPlantpid.T_ModelItem ml on ml.SP_ID=rp.SP_ModelItemID
						where ml.SP_ID in (select pll.sp_id from SPPIDTestPlantpid.T_PlantItem pll
												where pll.ItemTag like '%10"-105-DS-A1A%'))
AND mod.SP_ID = (select rpp.sp_modelitemid from SPPIDTestPlantpid.T_Representation rpp where rpp.SP_ID=con.SP_ConnectItem2ID)
and PR.SP_ID=rep.SP_ModelItemID;



-- ======================================================================================================
-- Get Drawing name from ItemTag
 select distinct dwg.Name, pl.ItemTag, dwg.Path
 from SPPIDTestPlantpid.T_Drawing dwg
 inner join SPPIDTestPlantpid.T_Representation rep on rep.SP_DrawingID=dwg.SP_ID
 inner join SPPIDTestPlantpid.T_PlantItem pl on rep.SP_ModelItemID=pl.SP_ID
 where pl.itemtag like '%10"-105-DS-A1A%';
 -- ======================================================================================================