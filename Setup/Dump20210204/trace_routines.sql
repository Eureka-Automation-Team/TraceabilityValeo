-- MySQL dump 10.13  Distrib 8.0.20, for Win64 (x86_64)
--
-- Host: localhost    Database: trace
-- ------------------------------------------------------
-- Server version	8.0.20

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Temporary view structure for view `logging_by_part_assemblies_v`
--

DROP TABLE IF EXISTS `logging_by_part_assemblies_v`;
/*!50001 DROP VIEW IF EXISTS `logging_by_part_assemblies_v`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `logging_by_part_assemblies_v` AS SELECT 
 1 AS `Id`,
 1 AS `Station`,
 1 AS `Machine`,
 1 AS `ItemCode`,
 1 AS `ModelRunning`,
 1 AS `PartSerialNumber`,
 1 AS `Actuator`,
 1 AS `ProductionDate`,
 1 AS `SwNumber`,
 1 AS `OpenAngle`,
 1 AS `LineErrorCounter`,
 1 AS `FinalResult`,
 1 AS `PartDescription`,
 1 AS `PartSerailNumber`*/;
SET character_set_client = @saved_cs_client;

--
-- Temporary view structure for view `logging_by_tightenings_v`
--

DROP TABLE IF EXISTS `logging_by_tightenings_v`;
/*!50001 DROP VIEW IF EXISTS `logging_by_tightenings_v`*/;
SET @saved_cs_client     = @@character_set_client;
/*!50503 SET character_set_client = utf8mb4 */;
/*!50001 CREATE VIEW `logging_by_tightenings_v` AS SELECT 
 1 AS `Id`,
 1 AS `Station`,
 1 AS `Machine`,
 1 AS `ItemCode`,
 1 AS `ModelRunning`,
 1 AS `PartSerialNumber`,
 1 AS `Actuator`,
 1 AS `ProductionDate`,
 1 AS `SwNumber`,
 1 AS `OpenAngle`,
 1 AS `LineErrorCounter`,
 1 AS `FinalResult`,
 1 AS `Desceiption`,
 1 AS `Min`,
 1 AS `Max`,
 1 AS `Target`,
 1 AS `Result`,
 1 AS `TestResult`*/;
SET character_set_client = @saved_cs_client;

--
-- Final view structure for view `logging_by_part_assemblies_v`
--

/*!50001 DROP VIEW IF EXISTS `logging_by_part_assemblies_v`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `logging_by_part_assemblies_v` AS select `log`.`Id` AS `Id`,`log`.`StationId` AS `Station`,`mac`.`ManchineName` AS `Machine`,`log`.`ItemCode` AS `ItemCode`,`log`.`ModelRunning` AS `ModelRunning`,`log`.`PartSerialNumber` AS `PartSerialNumber`,`log`.`Actuator` AS `Actuator`,`log`.`ProductionDate` AS `ProductionDate`,`log`.`SwNumber` AS `SwNumber`,`log`.`OpenAngle` AS `OpenAngle`,`log`.`LineErrorCounter` AS `LineErrorCounter`,if((`log`.`FinalResult` = 1),'OK','NOK') AS `FinalResult`,`part`.`PartName` AS `PartDescription`,`part`.`SerialNumber` AS `PartSerailNumber` from ((`traceability_logs` `log` left join `machines` `mac` on((`log`.`MachineId` = `mac`.`Id`))) left join `part_assemblies` `part` on((`log`.`Id` = `part`.`TraceabilityLogId`))) where (`log`.`Id` > 748) order by `log`.`ItemCode`,`log`.`MachineId`,`part`.`Id` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;

--
-- Final view structure for view `logging_by_tightenings_v`
--

/*!50001 DROP VIEW IF EXISTS `logging_by_tightenings_v`*/;
/*!50001 SET @saved_cs_client          = @@character_set_client */;
/*!50001 SET @saved_cs_results         = @@character_set_results */;
/*!50001 SET @saved_col_connection     = @@collation_connection */;
/*!50001 SET character_set_client      = utf8mb4 */;
/*!50001 SET character_set_results     = utf8mb4 */;
/*!50001 SET collation_connection      = utf8mb4_0900_ai_ci */;
/*!50001 CREATE ALGORITHM=UNDEFINED */
/*!50013 DEFINER=`root`@`localhost` SQL SECURITY DEFINER */
/*!50001 VIEW `logging_by_tightenings_v` AS select `log`.`Id` AS `Id`,`log`.`StationId` AS `Station`,`mac`.`ManchineName` AS `Machine`,`log`.`ItemCode` AS `ItemCode`,`log`.`ModelRunning` AS `ModelRunning`,`log`.`PartSerialNumber` AS `PartSerialNumber`,`log`.`Actuator` AS `Actuator`,`log`.`ProductionDate` AS `ProductionDate`,`log`.`SwNumber` AS `SwNumber`,`log`.`OpenAngle` AS `OpenAngle`,`log`.`LineErrorCounter` AS `LineErrorCounter`,if((`log`.`FinalResult` = 1),'OK','NOK') AS `FinalResult`,`result`.`No` AS `Desceiption`,`result`.`Min` AS `Min`,`result`.`Max` AS `Max`,`result`.`Target` AS `Target`,`result`.`Result` AS `Result`,`result`.`TestResult` AS `TestResult` from ((`traceability_logs` `log` left join `machines` `mac` on((`log`.`MachineId` = `mac`.`Id`))) left join `tightening_results` `result` on((`log`.`Id` = `result`.`TraceLogId`))) where (`log`.`Id` > 748) order by `log`.`ItemCode`,`log`.`MachineId`,`result`.`Id` */;
/*!50001 SET character_set_client      = @saved_cs_client */;
/*!50001 SET character_set_results     = @saved_cs_results */;
/*!50001 SET collation_connection      = @saved_col_connection */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-02-04 20:43:51
