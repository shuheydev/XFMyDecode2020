<img src='https://build.appcenter.ms/v0.1/apps/88827210-f6a1-4530-960a-78f583a3cdbb/branches/develop/badge' />

[![Build status](https://build.appcenter.ms/v0.1/apps/88827210-f6a1-4530-960a-78f583a3cdbb/branches/develop/badge)](https://appcenter.ms)

# このアプリについて
このアプリはde:code2020のセッションの検索,視聴をサポートする目的で作りました.

セッションを検索して,お気に入りに追加して,公式サイトをブラウザで開いて視聴する,という単純なものです.

<img src='https://user-images.githubusercontent.com/43431002/85307821-57b4c380-b4eb-11ea-8f1b-19199f0f48e3.png' width='200'/>
<img src='https://user-images.githubusercontent.com/43431002/85307927-83d04480-b4eb-11ea-948f-e85a069c8c86.png' width='200'/>

検索方式は単純な文字列検索です.

# 使い方

- 画面上部にある検索ボックスに検索したい単語を入力するとリストが絞り込まれます.
- スペースで区切った複数の単語を入力すると,すべての単語でAND検索します.
- 単語の前に｢-｣をつけると,その単語が含まれているセッションを除外します.
- お気に入り済みのセッションのみを検索したい場合は以下の文字列を入力します.
  - ｢★｣,｢☆｣,｢fav:｣,｢like:｣,｢star:｣
- お気に入り登録していないセッションのみを検索したい場合は上記文字列の前に｢-｣をつけます.
- Twitterアプリがインストールされていれば,そのセッション番号のハッシュタグ付きでTwitterを起動します.
- ｢#A｣,｢#B11｣などの文字列でトラックで絞り込むことができます.

![20200621_020634](https://user-images.githubusercontent.com/43431002/85207760-11822780-b366-11ea-8bdc-5998d8e8400f.gif)

# 利用できるプラットフォーム
Xamarin.Formsで作成したのでiOSでも動くと思いますが開発環境がないので検証できていません.

かといってAndroidで確実に動作するかというとこれも検証しておらず,少なくとも｢私のPixel 3aでは動く｣ことは確かめました.

# 環境
私の開発,実行環境は以下のとおりです.

- Windows 10 Pro 1904
- Visual Studio 2019 16.6.2
- Xamarin.Forms 4.7.098
- Google Pixel 3a Android 10

# 使用したライブラリなど
素晴らしいライブラリでした.

* [Xamarin.Forms](https://www.xamarin.com/forms)
* [Xamarin.Forms Material](https://docs.microsoft.com/xamarin/xamarin-forms/user-interface/visual/material-visual)
* [Xamarin.Essentials](https://www.github.com/xamarin/essentials)
* [Mvvm Helpers](https://github.com/jamesmontemagno/mvvm-helpers)
* [PancakeView](https://github.com/sthewissen/Xamarin.Forms.PancakeView)
* [Sharpnado - MaterialFrame & More](https://github.com/roubachof/Sharpnado.Presentation.Forms)
* [Syncfusion - ComboBox, MaskedEdit, NumericTextBox, Shimmer, Effects, TextInputLayout](https://www.syncfusion.com/xamarin)

# 使用したデータ
ありがとうございました.

- 公式サイト提供のJSONデータ
- Taiki Yoshidaさんが作成されたJSONデータ(https://github.com/taiki-yoshida/decode2020-powerapps)


# アップデート
## 20200623
- 視聴済みフラグを導入した
- ホーム,お気に入り,視聴済みをタブで切り替えできるようにした.
