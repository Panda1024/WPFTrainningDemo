   M S B A M L   `   `   `  ����G  CTranningDemo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:TranningDemo.ViewTranningDemo.View  5   	   &     TranningDemo.View.MainWindowView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:TranningDemo.View     5   	     � Title$=  7Examination room information management system_Fang,Li ��5   	   $	��450��6I   $	��800��6V    � 	Resources 5
       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $ ..\Theme/Style.xaml?�6%   %    5      .��5      �  � RowDefinitions 5   
   �� 5      $��40 �6   �� 5      �� 5      $��20 �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $��0,0,0,0q�6;   .+�5      �� $��
Search Bar��6   $
��Auto��6)   $
��Auto��67   $��10 5 5 5q�6D   $��Center=�6V   $
��Left=�6q   �� 5         defaultTextBoxStyle#��[  6   ��5      �� *	SearchKey+�� 5      $��Search��6     defaultButtonStyle#��[ 6&   $��10q�6R   ��5      �� *SearchCommand+�� 5       $��Clear��6   #��[ 6%   $��10q�6Q   ��5!      �� *ClearCommand+�� 5"      $	��Add��6   #��[ 6#   $��10q�6O   ��5#      �� *
AddCommand+5$      &   System.Windows.Controls.DataGrid  5'   
   -   5        $��DataDisplay��5'       $��1��6      ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ * 63      AutoGenerateColumns .     
IsReadOnly . 	   CanUserAddRows	 .  
   SelectionMode$
 Single=�5*      ��5)      �� *GridModelList+   Columns 5+      0  *System.Windows.Controls.DataGridTextColumn  5,      ,  &System.Windows.Controls.DataGridColumn   Header$ 	Class No.��6%   1  +System.Windows.Controls.DataGridBoundColumn   Binding 5-   %   �� *	ClassNo+  5.      $ Institute of Students��6%    5/   %   �� *InstituteStudents+  50      $ Number of Students��6%    51   %   �� *NumberStudents+  52      $ Institute of Proctors��6%    53   %   �� *InstituteProctors+  54      $ Number of Proctors��6%    55   %   �� *NumberProctors+4  .System.Windows.Controls.DataGridTemplateColumn  56      $ Operate��6)      CellTemplate 57      �� 58      .R�59      �� $ 
Horizontal=�6)   .+�5:   "   �� $
��Edit��6)   #��[ 68   ��5;   )   �� *DataContext.EditCommand+ �� RelativeSource ��  �� Mode$ FindAncestor=� �� AncestorType$ DataGridL���5<   )   ��  �� ElementName$ DataDisplay�� �� Path$ SelectedItem� �� Mode$ OneWay=��� 5=   "   $��Delete��6)   #��[ 6:   
���  ����5>   )   �� *DataContext.DeleteCommand+ �� $ FindAncestor=�$ DataGridL���5?   )   �� $ DataDisplay��$ SelectedItem�$ OneWay=�5@      5A      5C      5E      �� 5H   
   $��2��6   
�������$��25��69   $��Bottom=�6E   .+�5I      �� $
��Left=�62   $��Center=�6M   ��6   �� *	PrintText+6M   5J      5K      5M      