   M S B A M L   `   `   `  ����Q  MToolCalibWifiForGW040H, Version=1.0.0.4, Culture=neutral, PublicKeyToken=null?$clr-namespace:ToolCalibWifiForGW040HToolCalibWifiForGW040H  5   	   )    #ToolCalibWifiForGW040H.configWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   6local$clr-namespace:ToolCalibWifiForGW040H     5   	     � Title$  configWindow��5   	   $	��280��6   $	��450��6+    � WindowStartupLocation$ CenterScreen=�67      customWindow#��[  6\   .��5
      �� ���#BFCDDB �� CornerRadius$ 2��6"     colorBorder#��[ 63   $��1,1,1,1q�6^   ._�5   
   �  � RowDefinitions 5      �� 5      $��30 �6    �� 5      $��* �6    .+�5      � $��0��6   ���#FFFFFF.+�5      �� $��Center=�6   $
��Left=�63   
���   �$��12(�6a   $��Thiết lập cấu hình��6o   �� 5      -   5        $��50��5   4   $��Right=�6?   $��Center=�6b   $��Right=�6}   
���   �$��17(�6�   $��0,-5,5,0q�6�   $��X��6�   5      � 5      $��1��6   .+�5      �� $��5,10,0,0q�6   
���   �$	��0.5q�6A   $��Thiết lập ONT��6W   $	��225��6n   $	��130��6z   $	��Top=�6�   $
��Left=�6�   .��5      �� .+�5      � $��0,10,0,0q�6   .+�5      �� $
��Left=�6$   $��
IP Address��6?   �� 5      $��SemiBold"�6&   $��Center=�6<   $��Right=�6^   $	��150��6z   ��6�   ��  �� Path$ ONTIP� �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=�5      � 5      $��0,5,0,0q�6   .+�5      �� $
��Left=�6$   $
��User��6?   �� 5       $��SemiBold"�6&   $��Center=�6<   $��Right=�6^   $	��150��6z   ��6�   �� $ ONTUSER�$ PropertyChanged=�$ TwoWay=�5!      � 5"      $��0,5,0,0q�6   .+�5#      �� $
��Left=�6$   $��Password��6?   �� 5$      $��SemiBold"�6&   $��Center=�6<   $��Right=�6^   $	��150��6z   ��6�   �� $ ONTPASS�$ PropertyChanged=�$ TwoWay=�5%      5&      5'      � 5)      $	��230��6   $
��Left=�6(   $��	5,150,0,0q�6C   .+�5*      �� -   5        $��30��5*   2   $��90��6>   $��OK��6I   $��20,0,0,0q�6V   6j   �� 5+      -   5        $��30��5+   2   $��90��6>   $��Default��6I   $��20,0,0,0q�6[   6o   �� 5,      -   5        $��30��5,   2   $��90��6>   $��Cancel��6I   $��	70,10,0,0q�6Z   6o   5-      �� 5/      $��	5,10,10,0q�6   
���   �$	��0.5q�6B   $��Select Items��6X   $	��190��6n   $	��230��6z   $	��Top=�6�   $��Right=�6�   .��50      �� .+�51      �� -   5         ��Name$ spBefore��51   %   .+�52      �� $��0,10,0,0q�6�   $
��Left=�6�   $��Calibrate Tần Số��6�    d� 	IsChecked 6'   �� $ ENCALIBFREQ�$ PropertyChanged=�$ TwoWay=�6�   �� 53      $��0,10,0,0q�6�   $
��Left=�6�   $��Calibrate Power 2G��6�    6'   �� $ ENCALIBPW2G�$ PropertyChanged=�$ TwoWay=�6�   �� 54      $��0,10,0,0q�6�   $
��Left=�6�   $��Calibrate Power 5G��6�    6'   �� $ ENCALIBPW5G�$ PropertyChanged=�$ TwoWay=�6�   �� 55      $��0,10,0,0q�6   $
��Left=�6�   $��Write BIN registers��6�    6'   �� $ 
ENWRITEBIN�$ PropertyChanged=�$ TwoWay=�6�   �� 56      $��0,10,0,0q�6   $
��Left=�6�   $��Test Độ Nhạy Thu 2G��6�    6'   �� $ 
ENTESTRX2G�$ PropertyChanged=�$ TwoWay=�6�   �� 57      $��0,10,0,0q�6   $
��Left=�6�   $��Test Độ Nhạy Thu 5G��6�    6'   �� $ 
ENTESTRX5G�$ PropertyChanged=�$ TwoWay=�6�   �� 58      $��0,10,0,0q�6   $
��Left=�6�   $��Test Signal 2G��6�    6'   �� $ 
ENTESTTX2G�$ PropertyChanged=�$ TwoWay=�6�   �� 59      $��0,10,0,0q�6   $
��Left=�6�   $��Test Signal 5G��6�    6'   �� $ 
ENTESTTX5G�$ PropertyChanged=�$ TwoWay=�6�   5:      �� 5;      -   5        $ spAfter��5;   %   .+�5<      �� $��0,10,0,0q�6�   $
��Left=�6�   $��Test Anten1��6�    6'   �� $ ENTESTANTEN1�$ PropertyChanged=�$ TwoWay=�6�   �� 5=      $��0,10,0,0q�6�   $
��Left=�6�   $��Test Anten2��6�    6'   �� $ ENTESTANTEN2�$ PropertyChanged=�$ TwoWay=�6�   5>      5?      5A      5C      5E      5F      5G      