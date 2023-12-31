# VOIR簡易マニュアル
## VOIRとは
VOIRとは、Unity上で開発されたHMD型のバーチャルリアリティ(VR)装置用対話的可視化ソフトウェアである。このソフトウェアは、PCとHMDで実行することが可能であるにもかかわらず、CAVE装置用可視化ソフトウェアVFIVEと同等の対話的可視化を実現する。

VOIRはカーテシアン座標で定義された3次元のスカラーデータとベクトルデータが可視化できる。またデータ可視化時に、STL形式(バイナリ)のサーフェスや曲線も表示することが可能である。
VOIRの起動に必要なデータは、
1. スカラーデータ（最大3個）
2. ベクトルデータ（最大3個）
3. 座標データ（Rectilinearの場合）
4. 面データ（任意）
5. 線データ（任意）

である。
これらの情報は、VOIRの設定ファイル(JVファイル)に記述する。JVファイルについては、後述する。

スカラーデータとベクトルデータは、最低どちらか1個必要である。各データは最大3個入力できるが、voirConst.cs内の下記変数

> public const int MAXNVECT = 3;<br>
> public const int MAXNSCAL = 3;

を変更することで、最大個数を変更することもできる。

## データフォーマット
### スカラデータおよびベクトルデータ
単精度または倍精度実数のバイナリデータ（例えばFortranの書式なし出力）です。SIZEXをX方向、SIZEYをY方向、SIZEZをZ方向のグリッドサイズとすると
- C言語: double(or float) data[SIZEZ][SIZEY][SIZEX]
- Fortran: real*8(or real) data(SIZEX, SIZEY, SIZEZ)

である。ベクトルデータについては、X成分、Y成分、Z成分を別々に用意する。

### 座標データ (Rectilinearの場合)
座標データは、倍精度実数のバイナリデータで、これもX成分、Y成分、Z成分を別々に用意する。
- C言語: double x[SIZEX], double y[SIZEY], double z[SIZEZ]
- Fortran: real*8 x(SIZEX), y(SIZEY), z(SIZEZ)

## 設定ファイル(JVファイル)
VOIRの実行には、JVファイル(<u>J</u>e <u>V</u>ois)と呼ばれる設定ファイルが必要となる。JVファイルはテキストファイルである。以下に例を示す。'#'で始まる行は、コメントである。キーワードと値の間をスペースで区切る。

    # VOIR Config file (example.jv)
    #

    # x,y,z方向のグリッドサイズ
    GRIDSIZE   200 200 200

    # ベクトルデータの数
    NVEC    1
    # スカラーデータの数
    NSCAL   2

    #---------グリッドが等間隔の場合
    UNIFORM
    # x, y, z方向のグリッド間隔
    DX 0.005 0.005 0.005
    # x,y,zグリッドの端の位置
    CORNER -0.5 -0.5 -0.5
    
    #---------グリッドがRectilinearの場合、x,y,z方向の座標ファイルを用意する
    #COORDFILES   c:\Users\ohno\dynamo\dynamo.x c:\Users\ohno\dynamo\dynamo.y c:\Users\ohno\dynamo\dynamo.z

    # データファイル、座標ファイルの前後にヘッダ・フッタが4バイト付加されている場合(Fortranで出力した場合など)、4バイトスキップするよう指示する
    SKIP4BYTES

    # データファイルが単精度の場合
    SINGLEPRECISION

    #１個目のベクトルデータのラベル(メニューに表示される)。'@s'はスペースを意味する
    VECT0_LABEL  Velocity@sField 
    # 1個目のベクトルデータのX成分のファイル
    VECT0X  c:\Users\ohno\dynamo\dynamo.vel.xf
    # 1個目のベクトルデータのy成分のファイル
    VECT0Y  c:\Users\ohno\dynamo\dynamo.vel.yf
    # 1個目のベクトルデータのz成分のファイル
    VECT0Z  c:\Users\ohno\dynamo\dynamo.vel.zf

    #１個目のスカラーデータのラベル(メニューに表示される)。'@s'はスペースを意味する
    SCAL0_LABEL vorticity_z
    #１個目のスカラーデータのファイル
    SCAL0   c:\Users\ohno\dynamo\dynamo.vorticity_zf

    # 2個目のスカラーデータのラベル(メニューに表示される)。'@s'はスペースを意味する
    SCAL1_LABEL vor_z_sq
    # 2個目のスカラーデータのファイル
    SCAL1   c:\Users\ohno\dynamo\dynamo.vor_z_sqf

    # 曲線のファイル
    CURVEFILES c:\Users\ohno\dynamo\cindex.c c:\Users\ohno\dynamo\linex.c c:\Users\ohno\dynamo\liney.c c:\Users\ohno\dynamo\linez.c

    # サーフェスのファイル(STL形式)
    SURFACEFILES  c:\Users\ohno\dynamo\surfstl.stl 


## プログラムの実行
プログラムの実行は、JVファイルを引数として、コマンドプロンプトで

    > voir example.jv

と入力する。Unityエディタで実行する場合、JVファイルはvoirConst.csの

> public const string paramfile = "Assets/data/example.jv";

で指定する。

プログラムが実行されると、データの境界を示す赤(x方向)・緑(y方向)・青(z方向)の線が現れる。

## 操作方法
VOIR起動後は、左右のコントローラで操作を行う。
メニューから可視化したいデータ、ついで可視化手法を選ぶ。

### ジョイスティック
右コントローラのジョイスティックで、VR空間内を移動することができる。移動したい方向に右コントローラを向けて、ジョイスティックを前後に倒すと、その方向進む（後進する）。ジョイスティックを左右に倒すと、回転する。

### ボタン
左右のコントローラのトリガーボタンとグリップボタンを使用する。
右コントローラのトリガーボタンはメニューアイテムの選択と可視化パラメータの変更に使用し、グリップボタンは可視化オブジェクトの消去に使用する。左コントローラのトリガーボタンはメニューのオンオフ、グリップボタンは流線の全消去にしようする。

### メニュー
左コントローラのトリガーボタンを押すと表示される。右コントローラから放射される赤いレーザーポインタでアイテムを選び、右コントローラのトリガーボタンで選択する。サブメニューがある場合、右に表示されるので、順次選択する。例えば、等値面可視化を行う場合は、まずスカラー場、等値面可視化をしたいデータを選択、等値面、ワイヤフレームかサーフェス表示か、スカラデータで色付けするか否かを選択し、すべて決定すると等値面の計算が始まる。

### 可視化手法

|可視化手法| 使用法 |
|:---:|---|
|等値面<br>(スカラデータ)|等値面レベルは、右コントローラのトリガーボタンを押したまま、コントローラを上下に動かすことで変更できる。トリガーボタンを離すと確定され、等値面が再計算される。ワイヤフレーム表示やスカラ値での色付けも可能である。|
|スライス<br>(スカラデータ)|XY面、YZ面、XZ面でスライスできる。スライスの位置は右コントローラのトリガーボタンを押したまま、コントローラを上下・左右・奥手前に動かすことで変更できる。トリガーボタンを離すと確定され、スライス面が再計算される。ワイヤフレーム表示や凹凸付きスライスも可能である。|
|ローカルスライス<br>(スカラデータ)|右コントローラのトリガーボタンを押すと右コントローラからレーザーがでて、その先端に局所的なスライス面が表示される。ボタンを押したまま右コントローラを動かすことで、ユーザーの見たい位置に局所スライスを移動させることができる。|
|力線<br>(ベクトルデータ) &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;|右コントローラのトリガーボタンを押すと右コントローラからレーザーが出る。押した状態でコントローラを動かし力線計算を開始したい位置にレーザーの先を合わせ、トリガーボタンを離すと力線計算がスタートする。左コントローラのグリップボタンで直近の力線を消去、右コントローラのグリップボタンで全流線を消去できる。|
|ローカルアローズ<br>(ベクトルデータ)|右コントローラのトリガーボタンを押すとレーザーが出て、その先端の周りに矢印群が現れる。矢印群の位置は、トリガーボタンを押したまま右コントローラを動かすことで移動できる。|
|フラッシュライト<br>(ベクトルデータ)|右コントローラのトリガーボタンを押すと、右コントローラの前面のコーン状の領域内で、（懐中電灯で照らしたように）無数の粒子がベクトル場に沿って移動する。トリガーボタンを押したままコントローラを動かすとコーンの領域を移動させることができる。|
|ホタル(蛍)<br>(ベクトルデータ)|データ領域全体で、無数の粒子がベクトル場に沿って移動する。|


その他の機能
| 機能 | 使用法 |
|:---:|---|
|Snapshot|表示されている映像を画像として保存する機能。右コントローラのトリガボタンを押すと、表示されている映像がPNG形式で保存される。|
|ROI|より詳しく解析したい領域を選択する機能。右コントローラのトリガボタンを押して、選択したい領域の端点(始点)を設定、押したままコントローラを移動してトリガーボタンを離すともう一つの端点(終点)が設定される。二つの端点(始点と終点)で囲まれた直方体が新たな解析領域である。|
|オブジェクト &nbsp;|曲面、曲線の表示非表示を選択できる。|
|QUIT|VOIRの終了|

### 曲線のフォーマット
曲線表示には４つのファイルが必要である。すべてバイナリファイル（Fortranの書式なし）で用意する。
一つ目は、線分の数や頂点の数のデータである。
4バイト整数で

線分の数 各線分の頂点数

が記録されている。例えば、線分の数が3で、それぞれ超点数が10, 15, 14のとき、このファイルの内容は

    3 10 15 14

となる。
残りの三つは、各頂点のx座標、y座標、z座標を単精度実数で記録したファイルで、例えば(3.0, 5.2, 2.1), (3.5, 5.5, 2.3) ,...となっている場合は

    x座標のファイル: 3.0 3.5 ....
    y座標のファイル: 5.2 5.5 ....
    z座標のファイル: 2.1 2.3 ....

のように記録されている。