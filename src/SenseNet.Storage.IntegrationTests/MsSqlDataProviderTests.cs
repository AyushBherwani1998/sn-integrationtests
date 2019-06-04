﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SenseNet.ContentRepository;
using SenseNet.ContentRepository.Storage;
using SenseNet.ContentRepository.Storage.Data;
using SenseNet.ContentRepository.Storage.Data.MsSqlClient;
using SenseNet.ContentRepository.Storage.Schema;
using SenseNet.Tests.Implementations;
using SenseNet.Tests.Implementations2;
using Task = System.Threading.Tasks.Task;

namespace SenseNet.Storage.IntegrationTests
{
    [TestClass]
    public class MsSqlDataProviderTests : StorageTestBase
    {
        // ReSharper disable once InconsistentNaming
        private static MsSqlDataProvider DP => (MsSqlDataProvider)DataStore.DataProvider;
        // ReSharper disable once InconsistentNaming
        private static ITestingDataProviderExtension TDP => DataStore.GetDataProviderExtension<ITestingDataProviderExtension>();

        [TestMethod]
        public async Task MsSqlDP_InsertNode()
        {
            await StorageTest(false, async () =>
            {
                DataStore.Enabled = true;

                var root = CreateTestRoot();

                // Create a file but do not save.
                var created = new File(root) { Name = "File1", Index = 42, Description = "File1 Description" };
                created.Binary.SetStream(RepositoryTools.GetStreamFromString("File1 Content"));
                var nodeData = created.Data;
                nodeData.Path = RepositoryPath.Combine(created.ParentPath, created.Name);
                GenerateTestData(nodeData);


                // ACTION
                var nodeHeadData = nodeData.GetNodeHeadData();
                var versionData = nodeData.GetVersionData();
                var dynamicData = nodeData.GetDynamicData(false);
                var binaryProperty = dynamicData.BinaryProperties.First().Value;
                await DP.InsertNodeAsync(nodeHeadData, versionData, dynamicData);

                // ASSERT
                Assert.IsTrue(nodeHeadData.NodeId > 0);
                Assert.IsTrue(nodeHeadData.Timestamp > 0);
                Assert.IsTrue(versionData.VersionId > 0);
                Assert.IsTrue(versionData.Timestamp > 0);
                Assert.IsTrue(binaryProperty.Id > 0);
                Assert.IsTrue(binaryProperty.FileId > 0);
                Assert.IsTrue(nodeHeadData.LastMajorVersionId == versionData.VersionId);
                Assert.IsTrue(nodeHeadData.LastMajorVersionId == nodeHeadData.LastMinorVersionId);

                DistributedApplication.Cache.Reset();
                var loaded = Node.Load<File>(nodeHeadData.NodeId);
                Assert.IsNotNull(loaded);
                Assert.AreEqual("File1", loaded.Name);
                Assert.AreEqual(nodeHeadData.Path, loaded.Path);
                Assert.AreEqual(42, loaded.Index);
                Assert.AreEqual("File1 Content", RepositoryTools.GetStreamString(loaded.Binary.GetStream()));

                foreach (var propType in loaded.Data.PropertyTypes)
                    loaded.Data.GetDynamicRawData(propType);
                DataProviderChecker.Assert_DynamicPropertiesAreEqualExceptBinaries(nodeData, loaded.Data);

            });
        }
        [TestMethod]
        public async Task MsSqlDP_Update()
        {
            await StorageTest(false, async () =>
            {
                DataStore.Enabled = true;

                var root = CreateTestRoot();

                var created = new File(root) { Name = "File1", Index = 42 };
                created.Binary.SetStream(RepositoryTools.GetStreamFromString("File1 Content"));
                created.Save();

                // Update a file but do not save
                var updated = Node.Load<File>(created.Id);
                updated.Index = 142;
                updated.Binary.SetStream(RepositoryTools.GetStreamFromString("File1 Content UPDATED"));
                var nodeData = updated.Data;
                GenerateTestData(nodeData);

                // ACTION
                var nodeHeadData = nodeData.GetNodeHeadData();
                var versionData = nodeData.GetVersionData();
                var dynamicData = nodeData.GetDynamicData(false);
                var versionIdsToDelete = new int[0];
                //var binaryProperty = dynamicData.BinaryProperties.First().Value;
                await DP.UpdateNodeAsync(nodeHeadData, versionData, dynamicData, versionIdsToDelete);

                // ASSERT
                Assert.IsTrue(nodeHeadData.Timestamp > created.NodeTimestamp);

                DistributedApplication.Cache.Reset();
                var loaded = Node.Load<File>(nodeHeadData.NodeId);
                Assert.IsNotNull(loaded);
                Assert.AreEqual("File1", loaded.Name);
                Assert.AreEqual(nodeHeadData.Path, loaded.Path);
                Assert.AreEqual(142, loaded.Index);
                Assert.AreEqual("File1 Content UPDATED", RepositoryTools.GetStreamString(loaded.Binary.GetStream()));

                foreach (var propType in loaded.Data.PropertyTypes)
                    loaded.Data.GetDynamicRawData(propType);
                DataProviderChecker.Assert_DynamicPropertiesAreEqualExceptBinaries(nodeData, loaded.Data);
            });
        }

        [TestMethod]
        public async Task MsSqlDP_CopyAndUpdate_NewVersion()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_CopyAndUpdate_ExpectedVersion()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_UpdateNodeHead()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_HandleAllDynamicProps()
        {
            Assert.Inconclusive();
        }


        [TestMethod]
        public async Task MsSqlDP_Rename()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_LoadChildren()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_Move()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_RefreshCacheAfterSave()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_LazyLoadedBigText()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_LazyLoadedBigTextVsCache()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_LoadChildTypesToAllow()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_ContentListTypesInTree()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_ForceDelete()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_DeleteDeleted()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_GetVersionNumbers()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_GetVersionNumbers_MissingNode()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_LoadBinaryPropertyValues()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_NodeEnumerator()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_NameSuffix()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_TreeSize_Root()
        {
            await StorageTest(false, async () =>
            {
                // ACTION
                var size = await DP.GetTreeSizeAsync("/Root", true);

                // ASSERT
                var expectedSize = (long)await ExecuteScalarAsync("SELECT SUM(Size) FROM Files");
                Assert.AreEqual(expectedSize, size);
            });
        }
        [TestMethod]
        public async Task MsSqlDP_TreeSize_Subtree()
        {
            await StorageTest(false, async () =>
            {
                // ACTION
                var size = await DP.GetTreeSizeAsync("/Root/System/Schema/ContentTypes/GenericContent/Folder", true);

                // ASSERT
                var sql = @"SELECT SUM(Size) FROM Files f
    JOIN BinaryProperties b ON b.FileId = f.FileId
    JOIN Versions v ON v.VersionId = b.VersionId
    JOIN Nodes n on n.NodeId = v.NodeId
WHERE Path LIKE '/Root/System/Schema/ContentTypes/GenericContent/Folder%'";
                var expectedSize = (long) await ExecuteScalarAsync(sql);
                Assert.AreEqual(expectedSize, size);
            });
        }
        [TestMethod]
        public async Task MsSqlDP_TreeSize_Item()
        {
            await StorageTest(false, async () =>
            {
                // ACTION
                var size = await DP.GetTreeSizeAsync("/Root/System/Schema/ContentTypes/GenericContent/Folder", false);

                // ASSERT
                var sql = @"SELECT SUM(Size) FROM Files f
    JOIN BinaryProperties b ON b.FileId = f.FileId
    JOIN Versions v ON v.VersionId = b.VersionId
    JOIN Nodes n on n.NodeId = v.NodeId
WHERE Path = '/Root/System/Schema/ContentTypes/GenericContent/Folder'";
                var expectedSize = (long) await ExecuteScalarAsync(sql);
                Assert.AreEqual(expectedSize, size);
            });
        }

        /* ================================================================================================== NodeQuery */

        [TestMethod]
        public async Task MsSqlDP_NodeQuery_InstanceCount()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_NodeQuery_ChildrenIdentfiers()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_NodeQuery_QueryNodesByTypeAndPathAndName()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_NodeQuery_QueryNodesByTypeAndPathAndProperty()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_NodeQuery_QueryNodesByReferenceAndType()
        {
            Assert.Inconclusive();
        }

        /* ================================================================================================== TreeLock */

        [TestMethod]
        public async Task MsSqlDP_LoadEntityTree()
        {
            Assert.Inconclusive();
        }

        /* ================================================================================================== TreeLock */

        [TestMethod]
        public async Task MsSqlDP_TreeLock()
        {
            Assert.Inconclusive();
        }

        /* ================================================================================================== IndexDocument */

        [TestMethod]
        public async Task MsSqlDP_LoadIndexDocuments()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_SaveIndexDocumentById()
        {
            Assert.Inconclusive();
        }

        /* ================================================================================================== IndexingActivities */

        [TestMethod]
        public async Task MsSqlDP_IndexingActivity_GetLastIndexingActivityId()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_IndexingActivity_LoadIndexingActivities_Page()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_IndexingActivity_LoadIndexingActivities_PageUnprocessed()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_IndexingActivity_LoadIndexingActivities_Gaps()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_IndexingActivity_LoadIndexingActivities_Executable()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_IndexingActivity_UpdateRunningState()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_IndexingActivity_RefreshLockTime()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_IndexingActivity_DeleteFinished()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_IndexingActivity_LoadFull()
        {
            Assert.Inconclusive();
        }

        /* ================================================================================================== Nodes */

        [TestMethod]
        public async Task MsSqlDP_CopyAndUpdateNode_Rename()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_LoadNodes()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_LoadNodeHeadByVersionId_Missing()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_NodeAndVersion_CountsAndTimestamps()
        {
            Assert.Inconclusive();
        }

        /* ================================================================================================== Errors */

        [TestMethod]
        public async Task MsSqlDP_Error_InsertNode_AlreadyExists()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_Error_UpdateNode_Deleted()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Error_UpdateNode_MissingVersion()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Error_UpdateNode_OutOfDate()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_Error_CopyAndUpdateNode_Deleted()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Error_CopyAndUpdateNode_MissingVersion()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Error_CopyAndUpdateNode_OutOfDate()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_Error_UpdateNodeHead_Deleted()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Error_UpdateNodeHead_OutOfDate()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_Error_DeleteNode()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_Error_MoveNode_MissingSource()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Error_MoveNode_MissingTarget()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Error_MoveNode_OutOfDate()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public async Task MsSqlDP_Error_QueryNodesByReferenceAndTypeAsync()
        {
            Assert.Inconclusive();
        }

        /* ================================================================================================== Transaction */

        [TestMethod]
        public async Task MsSqlDP_Transaction_InsertNode()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Transaction_UpdateNode()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Transaction_CopyAndUpdateNode()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Transaction_UpdateNodeHead()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Transaction_MoveNode()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Transaction_RenameNode()
        {
            Assert.Inconclusive();
        }
        [TestMethod]
        public async Task MsSqlDP_Transaction_DeleteNode()
        {
            Assert.Inconclusive();
        }

        /* ================================================================================================== Schema */

        [TestMethod]
        public async Task MsSqlDP_Schema_()
        {
            Assert.Inconclusive();
        }

        /* ================================================================================================== */

        private void GenerateTestData(NodeData nodeData)
        {
            foreach (var propType in nodeData.PropertyTypes)
            {
                var data = GetTestData(propType);
                if (data != null)
                    nodeData.SetDynamicRawData(propType, data);
            }
        }
        private object GetTestData(PropertyType propType)
        {
            if (propType.Name == "AspectData")
                return "<AspectData />";
            switch (propType.DataType)
            {
                case DataType.String: return "String " + Guid.NewGuid();
                case DataType.Text: return "Text value" + Guid.NewGuid();
                case DataType.Int: return Rng();
                case DataType.Currency: return (decimal)Rng();
                case DataType.DateTime: return DateTime.UtcNow;
                case DataType.Reference: return new List<int> { Rng(), Rng() };
                case DataType.Binary:
                default:
                    return null;
            }
        }
        private Random _random = new Random();
        private int Rng()
        {
            return _random.Next(1, int.MaxValue);
        }

        private SystemFolder CreateTestRoot()
        {
            return CreateFolder(Repository.Root, "TestRoot" + Guid.NewGuid());
        }
        private SystemFolder CreateFolder(Node parent, string name)
        {
            var folder = new SystemFolder(parent) { Name = name };
            folder.Save();
            return folder;
        }
        private File CreateFile(Node parent, string name, string fileContent)
        {
            var file = new File(parent) { Name = name };
            file.Binary.SetStream(RepositoryTools.GetStreamFromString(fileContent));
            file.Save();
            return file;
        }

        private async Task<object> ExecuteScalarAsync(string sql)
        {
            using (var ctx = new SnDataContext(DP))
                return await ctx.ExecuteScalarAsync(sql);
        }
    }
}
