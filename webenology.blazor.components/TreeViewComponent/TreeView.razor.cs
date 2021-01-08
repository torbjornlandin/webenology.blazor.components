﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

using Microsoft.AspNetCore.Components;

namespace webenology.blazor.components
{
    public partial class TreeView
    {
        [Parameter]
        public bool AllowSelectAll { get; set; }
        [Parameter]
        public bool StartExpended { get; set; }
        [Parameter]
        public List<TreeNode> TreeNodes { get; set; }
        [Parameter] public bool Selectable { get; set; } = true;
        [Parameter]
        public TreeViewStyle CssStyle { get; set; } = TreeViewStyle.WebenologyStyle;
        [Parameter]
        public EventCallback<List<string>> OnNodeSelectionChange { get; set; }

        private bool _toggleAll;
        private void ToggleAll()
        {
            _toggleAll = !_toggleAll;
            TreeNodes.ToggleCheck(_toggleAll);
        }

        public void ChangeNodeSelection()
        {
            StateHasChanged();
            var selectedNodes = callNodeSelectionChange(TreeNodes, new List<string>());
            OnNodeSelectionChange.InvokeAsync(selectedNodes);

            var allSelected = TreeNodes.AreAllSelected(new List<bool>());
            if (allSelected)
            {
                _toggleAll = true;
            }
            else
            {
                _toggleAll = false;
            }
            StateHasChanged();
        }

        private List<string> callNodeSelectionChange(List<TreeNode> nodes, List<string> selectedNodes)
        {

            foreach (var treeNode in nodes)
            {
                if (treeNode.Nodes.Any() && treeNode.Nodes.AreAllSelected(new List<bool>()))
                {
                    treeNode.IsSelected = true;
                }

                if (treeNode.IsSelected)
                {
                    selectedNodes.Add(treeNode.Node);
                    selectedNodes = callNodeSelectionChange(treeNode.Nodes, selectedNodes);
                }
            }

            return selectedNodes;
        }
    }
}
