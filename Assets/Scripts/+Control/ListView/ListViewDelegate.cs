using UnityEngine;
using System.Collections;

public interface IListViewDataSource
{
	int numberOfCells(ListView _listView);

	int templateForCell(ListView _listView, int _index);
	
}

public interface IListViewCell
{
	void OnDeselected();
	void OnSelected();
}

public interface ListViewCellDelegate
{
	void didSelectedCell(ListViewCell _cellInfo);
	void beginResizing(ListViewCell _cellInfo);
	void endResizing(ListViewCell _cellInfo);
}

public interface IListViewDelegate
{
	void reloadListView(ListView _listView);
	void didSelectedItem(ListView _listView, ListViewCell _cell);
	void didUnSelectedItem(ListView _listView, ListViewCell _cell);
}