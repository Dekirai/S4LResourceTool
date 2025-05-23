﻿namespace S4LResourceTool
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ColumnHeader columnHeader1;
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("_resources");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.bt_ChangePath = new System.Windows.Forms.Button();
            this.resourceList = new System.Windows.Forms.ListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewItemCtx = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tb_searchResource = new System.Windows.Forms.TextBox();
            this.textDisplay = new System.Windows.Forms.TextBox();
            this.tree = new System.Windows.Forms.TreeView();
            this.bt_Save = new System.Windows.Forms.Button();
            this.bt_FindUnused = new System.Windows.Forms.Button();
            this.imageDisplay = new System.Windows.Forms.PictureBox();
            this.progressBarSave = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.imageDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 160;
            // 
            // bt_ChangePath
            // 
            this.bt_ChangePath.Location = new System.Drawing.Point(12, 418);
            this.bt_ChangePath.Name = "bt_ChangePath";
            this.bt_ChangePath.Size = new System.Drawing.Size(191, 23);
            this.bt_ChangePath.TabIndex = 0;
            this.bt_ChangePath.Text = "Change client path";
            this.bt_ChangePath.UseVisualStyleBackColor = true;
            this.bt_ChangePath.Click += new System.EventHandler(this.button1_Click);
            // 
            // resourceList
            // 
            this.resourceList.AllowColumnReorder = true;
            this.resourceList.AllowDrop = true;
            this.resourceList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.resourceList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.resourceList.ContextMenuStrip = this.listViewItemCtx;
            this.resourceList.FullRowSelect = true;
            this.resourceList.HideSelection = false;
            this.resourceList.Location = new System.Drawing.Point(209, 3);
            this.resourceList.Name = "resourceList";
            this.resourceList.Size = new System.Drawing.Size(512, 409);
            this.resourceList.TabIndex = 3;
            this.resourceList.UseCompatibleStateImageBehavior = false;
            this.resourceList.View = System.Windows.Forms.View.Details;
            this.resourceList.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.resourceList.DragDrop += new System.Windows.Forms.DragEventHandler(this.listView1_DragDrop);
            this.resourceList.DragEnter += new System.Windows.Forms.DragEventHandler(this.listView1_DragEnter);
            this.resourceList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseClick);
            this.resourceList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Date modified";
            this.columnHeader2.Width = 130;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Checksum";
            this.columnHeader3.Width = 110;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Size";
            this.columnHeader4.Width = 65;
            // 
            // listViewItemCtx
            // 
            this.listViewItemCtx.Name = "listViewItemCtx";
            this.listViewItemCtx.Size = new System.Drawing.Size(61, 4);
            // 
            // tb_searchResource
            // 
            this.tb_searchResource.ForeColor = System.Drawing.Color.Silver;
            this.tb_searchResource.Location = new System.Drawing.Point(588, 418);
            this.tb_searchResource.Name = "tb_searchResource";
            this.tb_searchResource.Size = new System.Drawing.Size(133, 20);
            this.tb_searchResource.TabIndex = 4;
            this.tb_searchResource.Text = "Search resource..";
            this.tb_searchResource.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            this.tb_searchResource.Enter += new System.EventHandler(this.searchBox_Enter);
            this.tb_searchResource.Leave += new System.EventHandler(this.searchBox_Leave);
            // 
            // textDisplay
            // 
            this.textDisplay.AcceptsReturn = true;
            this.textDisplay.AcceptsTab = true;
            this.textDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textDisplay.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textDisplay.Location = new System.Drawing.Point(727, 3);
            this.textDisplay.Multiline = true;
            this.textDisplay.Name = "textDisplay";
            this.textDisplay.ReadOnly = true;
            this.textDisplay.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textDisplay.Size = new System.Drawing.Size(449, 409);
            this.textDisplay.TabIndex = 5;
            this.textDisplay.Visible = false;
            this.textDisplay.WordWrap = false;
            // 
            // tree
            // 
            this.tree.AllowDrop = true;
            this.tree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tree.FullRowSelect = true;
            this.tree.HotTracking = true;
            this.tree.ItemHeight = 28;
            this.tree.Location = new System.Drawing.Point(12, 3);
            this.tree.Name = "tree";
            treeNode1.Name = "_resources";
            treeNode1.Text = "_resources";
            this.tree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.tree.ShowLines = false;
            this.tree.Size = new System.Drawing.Size(191, 409);
            this.tree.TabIndex = 6;
            this.tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
            // 
            // bt_Save
            // 
            this.bt_Save.Location = new System.Drawing.Point(209, 418);
            this.bt_Save.Name = "bt_Save";
            this.bt_Save.Size = new System.Drawing.Size(148, 23);
            this.bt_Save.TabIndex = 8;
            this.bt_Save.Text = "Save changes";
            this.bt_Save.UseVisualStyleBackColor = true;
            this.bt_Save.Click += new System.EventHandler(this.bt_Save_Click);
            // 
            // bt_FindUnused
            // 
            this.bt_FindUnused.Location = new System.Drawing.Point(363, 418);
            this.bt_FindUnused.Name = "bt_FindUnused";
            this.bt_FindUnused.Size = new System.Drawing.Size(148, 23);
            this.bt_FindUnused.TabIndex = 9;
            this.bt_FindUnused.Text = "Find unused Resources";
            this.bt_FindUnused.UseVisualStyleBackColor = true;
            this.bt_FindUnused.Click += new System.EventHandler(this.bt_FindUnsed_Click);
            // 
            // imageDisplay
            // 
            this.imageDisplay.Location = new System.Drawing.Point(727, 3);
            this.imageDisplay.Name = "imageDisplay";
            this.imageDisplay.Size = new System.Drawing.Size(449, 409);
            this.imageDisplay.TabIndex = 10;
            this.imageDisplay.TabStop = false;
            this.imageDisplay.Visible = false;
            // 
            // progressBarSave
            // 
            this.progressBarSave.Location = new System.Drawing.Point(797, 418);
            this.progressBarSave.Name = "progressBarSave";
            this.progressBarSave.Size = new System.Drawing.Size(379, 23);
            this.progressBarSave.TabIndex = 11;
            this.progressBarSave.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(727, 423);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Saving files:";
            this.label1.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1188, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.progressBarSave);
            this.Controls.Add(this.imageDisplay);
            this.Controls.Add(this.bt_FindUnused);
            this.Controls.Add(this.bt_Save);
            this.Controls.Add(this.tree);
            this.Controls.Add(this.textDisplay);
            this.Controls.Add(this.tb_searchResource);
            this.Controls.Add(this.resourceList);
            this.Controls.Add(this.bt_ChangePath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Resource Tool by Dekirai";
            ((System.ComponentModel.ISupportInitialize)(this.imageDisplay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bt_ChangePath;
        private System.Windows.Forms.ListView resourceList;
        private System.Windows.Forms.TextBox tb_searchResource;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TextBox textDisplay;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.TreeView tree;
        private System.Windows.Forms.Button bt_Save;
        private System.Windows.Forms.ContextMenuStrip listViewItemCtx;
        private System.Windows.Forms.Button bt_FindUnused;
        private System.Windows.Forms.PictureBox imageDisplay;
        private System.Windows.Forms.ProgressBar progressBarSave;
        private System.Windows.Forms.Label label1;
    }
}

