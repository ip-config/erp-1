﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using TIV.Core.DatabaseAccess;
using MainProgram.model;
using MainProgram.bus;

namespace MainProgram
{
    using DatabaseAccessFactoryInstance = TIV.Core.BaseCoreLib.Singleton<DatabaseFactory>;

    public partial class FormMaterielTypeModify : Form
    {
        private bool m_isSave = false;
        private int m_materielGroupPkey = 1;
        private string m_materielGroupName = "";
        private int m_type = 1;

        private TreeNode m_rootNode;
        private TreeViewExtend m_tree;

        private DataGridViewExtend m_dataGridViewExtend = new DataGridViewExtend();


        public FormMaterielTypeModify(int type = 1)
        {
            InitializeComponent();

            m_type = type;

            if (m_type == 1)
            {
                this.Text = "物料分组调整";
            }
            else if (m_type == 2)
            {
                this.Text = "客户分组调整";
            }
            else if (m_type == 3)
            {
                this.Text = "供应商分组调整";
            }
        }

        private void FormBaseMateriel_Load(object sender, EventArgs e)
        {
            // 树控件初始化
            refreshTreeView();
        }

        private void refreshTreeView()
        {
            if (m_tree !=null && treeViewMaterielOrg.Nodes.Count > 0)
            {
                treeViewMaterielOrg.Nodes.Clear();
            }

            m_tree = new TreeViewExtend(this.treeViewMaterielOrg);

            if (m_type == 1)
            {
                int rootNodePkey = MaterielOrgStruct.getInctance().getRootNodePkey();

                int rooNodeValue = MaterielOrgStruct.getInctance().getNoteValueFromPkey(rootNodePkey);
                string rootName = MaterielType.getInctance().getMaterielTypeNameFromPkey(rooNodeValue);

                m_rootNode = this.treeViewMaterielOrg.Nodes.Add(Convert.ToString(rooNodeValue), rootName);

                processDutyOrgNodeMateriel(rootNodePkey, m_rootNode);
            }
            else if (m_type == 2)
            {
                int rootNodePkey = CustomerOrgStruct.getInctance().getRootNodePkey();

                int rooNodeValue = CustomerOrgStruct.getInctance().getNoteValueFromPkey(rootNodePkey);
                string rootName = CustomerType.getInctance().getCustomerTypeNameFromPkey(rooNodeValue);

                m_rootNode = this.treeViewMaterielOrg.Nodes.Add(Convert.ToString(rooNodeValue), rootName);

                processDutyOrgNodeCustomer(rootNodePkey, m_rootNode);
            }
            else if (m_type == 3)
            {
                int rootNodePkey = SupplierOrgStruct.getInctance().getRootNodePkey();

                int rooNodeValue = SupplierOrgStruct.getInctance().getNoteValueFromPkey(rootNodePkey);
                string rootName = SupplierType.getInctance().getSupplierTypeNameFromPkey(rooNodeValue);

                m_rootNode = this.treeViewMaterielOrg.Nodes.Add(Convert.ToString(rooNodeValue), rootName);

                processDutyOrgNodeSupplier(rootNodePkey, m_rootNode);

            }

            m_rootNode.Expand();
        }

        private void processDutyOrgNodeSupplier(int parentID, TreeNode node)
        {
            string nodeName = "";
            TreeNode currentNode;
            ArrayList nodeList = SupplierOrgStruct.getInctance().getNodesFormParentID(parentID);

            for (int i = 0; i < nodeList.Count; i++)
            {
                SupplierOrgStructTable record = (SupplierOrgStructTable)nodeList[i];

                nodeName = SupplierType.getInctance().getSupplierTypeNameFromPkey(record.value);

                currentNode = m_tree.addNode(node, nodeName, 0, 1, Convert.ToString(record.value));
                processDutyOrgNodeSupplier(record.pkey, currentNode);
            }
        }

        private void processDutyOrgNodeCustomer(int parentID, TreeNode node)
        {
            string nodeName = "";
            TreeNode currentNode;
            ArrayList nodeList = CustomerOrgStruct.getInctance().getNodesFormParentID(parentID);

            for (int i = 0; i < nodeList.Count; i++)
            {
                CustomerOrgStructTable record = (CustomerOrgStructTable)nodeList[i];

                nodeName = CustomerType.getInctance().getCustomerTypeNameFromPkey(record.value);

                currentNode = m_tree.addNode(node, nodeName, 0, 1, Convert.ToString(record.value));
                processDutyOrgNodeCustomer(record.pkey, currentNode);
            }
        }

        private void processDutyOrgNodeMateriel(int parentID, TreeNode node)
        {
            string nodeName = "";
            TreeNode currentNode;
            ArrayList nodeList = MaterielOrgStruct.getInctance().getNodesFormParentID(parentID);

            for (int i = 0; i < nodeList.Count; i++)
            {
                MaterielOrgStructTable record = (MaterielOrgStructTable)nodeList[i];

                nodeName = MaterielType.getInctance().getMaterielTypeNameFromPkey(record.value);

                string groupNum = MaterielType.getInctance().getMaterielTypeNumFromPkey(record.value);

                if (groupNum.Length > 0)
                {
                    nodeName += "(" + groupNum + ")";
                }

                currentNode = m_tree.addNode(node, nodeName, 0, 1, Convert.ToString(record.value));
                processDutyOrgNodeMateriel(record.pkey, currentNode);
            }
        }

        private void treeViewMaterielOrg_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.treeViewMaterielOrg.SelectedNode != null && this.treeViewMaterielOrg.SelectedNode.Nodes.Count >= 0)
            {
                this.treeViewMaterielOrg.SelectedNode.BackColor = Color.LightSteelBlue;

                // 单击鼠标时，得到当前树节点Pkey及文本值
                if (Convert.ToInt32(this.treeViewMaterielOrg.SelectedNode.Name.ToString()) != m_materielGroupPkey)
                {
                    m_materielGroupPkey = Convert.ToInt32(this.treeViewMaterielOrg.SelectedNode.Name.ToString());
                    m_materielGroupName = this.treeViewMaterielOrg.SelectedNode.Text.ToString();
                }
            }
            else 
            {
                m_materielGroupPkey = 0;
            }
        }

        private void treeViewMaterielOrg_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (this.treeViewMaterielOrg.SelectedNode != null && this.treeViewMaterielOrg.SelectedNode.Nodes.Count >= 0)
            {
                this.treeViewMaterielOrg.SelectedNode.BackColor = Color.Empty;
            }
        }

        //private void SuppierGroupDelete_Click(object sender, EventArgs e)
        //{
        //    if (MessageBoxExtend.messageQuestion("确定删除[" + m_materielGroupName + "]分组信息吗?"))
        //    {
        //        if (m_materielRecordCount > 0)
        //        {
        //            MessageBoxExtend.messageWarning("[" + m_materielGroupName + "] 删除失败,请先删除物料记录然后重试.");
        //        }
        //        else
        //        {
        //            MaterielType.getInctance().delete(m_materielGroupPkey);
        //            MaterielOrgStruct.getInctance().delete(MaterielOrgStruct.getInctance().getPkeyFromValue(m_materielGroupPkey));
        //            refreshTreeView();
        //        }
        //    }
        //}

        //private SortedDictionary<int, MaterielTable> getCurrentNodeAllChildNodesMateriel()
        //{
        //    SortedDictionary<int, MaterielTable> materielList = new SortedDictionary<int, MaterielTable>();
        //    SortedDictionary<int, int> childNodeValues = MaterielOrgStruct.getInctance().getAllChildNodeValue(m_materielGroupPkey);

        //    if (!childNodeValues.ContainsKey(m_materielGroupPkey))
        //    {
        //        childNodeValues.Add(m_materielGroupPkey, m_materielGroupPkey);
        //    }

        //    foreach (KeyValuePair<int, int> index in childNodeValues)
        //    {
        //        SortedDictionary<int, MaterielTable> temp = Materiel.getInctance().getMaterielInfoFromMaterielType(index.Value);

        //        foreach (KeyValuePair<int, MaterielTable> i in temp)
        //        {
        //            MaterielTable materiel = new MaterielTable();
        //            materielList.Add(materielList.Count, (MaterielTable)i.Value);
        //        }
        //    }

        //    return materielList;
        //}

        private void buttonClose_Click(object sender, EventArgs e)
        {
            m_isSave = false;
            this.Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            m_isSave = true;
            this.Close();
        }

        public int getSelectRecordPkey()
        {
            return m_materielGroupPkey;
        }

        public string getSelectTypeName()
        {
            return m_materielGroupName;
        }

        public bool getIsSave()
        {
            return m_isSave;
        }
    }
}