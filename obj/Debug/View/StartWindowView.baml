   M S B A M L   `   `   `  ����G  CTranningDemo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:TranningDemo.ViewTranningDemo.View  5   	   '    !TranningDemo.View.StartWindowView   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:TranningDemo.View     5   	     � Title$  StartWindowView��5   	   $	��300��6!   $	��400��6.    � 	Resources 5
       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $ ..\Theme/Style.xaml?�6%   %    5       � DataContext 5      �� 5   
    �� Path$ Start�6      Locator �� Source# [  6   6C   .��5      �  � RowDefinitions 5   
   �� 5      $��15 �6   �� 5      $��* �6   �� 5      $��* �6   �� 5      $��* �6   .+�5   
   o� $��0��6   $
��Auto��6   $��20��6)   
�������$	��Top=�6L   .@�5!      m� $
��File��6   .@�5"      m� $
��Open��6    m� Command 6)   �� *OpenFileCommand+m� 5#      $��Import xml file to SQL��6    6;   �� *ImportDataCommand+m� 5$      $
��Save��6    6)   �� *SaveFileCommand+5%      m� 5&      $��Setting��6   .@�5'      m� $��xml File��6    6-   �� *SetMode11Command+m� 5(      $��	json File��6    6.   �� *SetMode12Command+m� 5)      $��
PostgreSQL��6    6/   �� *SetMode13Command+5*      m� 5+      $��About��6   5,      m� 5-      $
��Help��6   5.      5/      �� 51   
   $��2��6   $��1��6   $	��150��6/   $��Start Main window��52      ��53      �� *StartMainCommand+55      56      