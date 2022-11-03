﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.4.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "8c66306bd3e888aee1a5d95fbd79a34c7406adc39455a0f1e469b4e2b8f02a0b6be6ea67e2f8d7f733f316f233301941ff1ec85d1af3619b985eaa250029034a" },
                { "ar", "d3a4039ce74eee9640df55975d65596187b29f862a0f7075d70ff2ff3c47680b561bfa35f1ed8dad0da1907046fc68287841feaeb0b50639ee9874d2a0c876af" },
                { "ast", "d7a2632089442c388f2d6a88963b5b9c61013a83a48991f46bbeaee3c012748cbe2a4a1578be2eefdd0d51b23f0ac493c1a05586527abfdaf4dfc144828c7c09" },
                { "be", "91d6f46917b4fedc24425aa356c91e0e5a28d73be669ef13fc895cc191c54d5365031f5177922b889996a4974f13decfb4dd528edaa5256e5669a84aa80dd597" },
                { "bg", "24253069c03ae1bd23413845633606a875b57ab009112eea0c0c7b6cbf8cf919e3af9a47f1cfd3bedba225548885a6d7979bcef9f3629fd793fc018f393a1ec0" },
                { "br", "ac17944983a7057a0112c26f84ceefc05bb9afbafea1cc63dfe3770b6f851b6e373c66bf52f71f6e3ec2148dde63bd87f53142fbee9c8d1b5f64c4839d1e50fb" },
                { "ca", "3837830fb250d754b85891db172487f7285d5ccecb9974c25584209d1e31e3d0e689347a20512697f70478a2150586866e3bf08d383925662cf9c25a4fc28497" },
                { "cak", "92f7d7cbc960747e00e987c5c1c0f8cf46c81a246e177ee701d2f4d1cca7622d52a64f9ac185bcfb8b721b6e1028d915cb6cf23fe89dc22dd6a30f6645d35257" },
                { "cs", "fa9720a68582652c71ad0cef39f4c21e617c6195005a82a86dc3cb1e2b248e0a9531c5c8da89e43ca16b99990fa1e7f5480fb5a044dea91f21c53a0758c922cf" },
                { "cy", "fcccb9f44ee2544631492f59da1ff58213c4bb641c9d190a3034db4a7ab47456d354e8a2993b7422c430e7e55dc6e8b55e741b83b3e2b39b8cca747c62200bf7" },
                { "da", "8c618a41d81f2b5195dba21328f3cee860d458a003c051c6a1b3504b3196deaacb14aec782e859660ba3eb19f73e97de86bbf196637e6ddda7c9d6d90a7008b7" },
                { "de", "f5009a5fe44367414cbcdee6ee927b4f4afc6baf5a7ce167906ed0fa56985b76738bde4012c147eaf2a0a627f89212193f3bbb3592f721fd4db36ae4676d53b8" },
                { "dsb", "6d2afa11eb2b1e9769023bde88b458b9a5a106aaff6f58b9b54a60fa51171dfb802f8c46350663f3d258c43297491e65a32b56e2d5b2b55e4b91cce04c94ba25" },
                { "el", "6a47b109a7d08568e74d85c9416bfdee2d2b24b6c10fe6a5fc83d6dc691609ebbbca42dd2a89c445185397c5085a0e9092eec6ef17c490885001ce494e4cdecf" },
                { "en-CA", "253a9b8a7bc1ca553039cbe3db6921b2ceae92f186363f3ef2cde80f48edcb5055cd8bae131ce9df8347187b916b7a259f65cab4edc83f3711613d551bd57e29" },
                { "en-GB", "213fceece8c168c1e3d512b8a3ad4daa77c3d4b5f4ea7ef034e131f7e47f28a99851159f11edae225c4f4fc1bc14ac199cce933b6283681c007abb29339605b4" },
                { "en-US", "61a812338e2d446858a21ebce80af564313b35efa8266e024a87370461beeb48e3923176c6be6cb3e45050b460d498cec54f767287c50313d5c8395e0c38ae1a" },
                { "es-AR", "ec1b24f0456b2095048f1c7ddfa92bb920677ec48747fa16e2bd021ff51036536192b9cf03cad170d9e1718f6b71d56707fd65633bdf3607e69bac2d6b280bd9" },
                { "es-ES", "5a1bc9f03ee08e74a9476e20a716029940dfdd8fb14c18be0045b81f7757c23bdc8de1ca727e30a82755ebe184968e5fad26e4e8b1e1bc4694f6d4dad2cadc0b" },
                { "es-MX", "360884248ea41074d1cf5de135babea1448039cce748df4cff8f351728bceb4eab01fb3d09e0b89674fc5fcedfe27a5830fbf43cdd7ff5b0b6e97931eb9ec318" },
                { "et", "bc1248c94893cda9f44f29ff8bf942edde7879307cdc7d570663ed126b9b1abb16ea1d702779689ba7f186edd1201ea2e44dc8ec83da3936663acf22cea53d15" },
                { "eu", "023d5f9d4ddb74693cb49aafa4f931517baddc968ad23dcc401c92a6f5a242a68afbcdd53a4eee4ed797b457a4f23fbb57db9dc9e21db7ada2ec7a132558455d" },
                { "fi", "40647027df089cb165b8444378fba88219a3fb7538c3e760ee8d0f347e64ca5e1c807bdcbe83178d4f4068aecbce16bd59d6d402780e8812887ca9579ee64651" },
                { "fr", "7b85e2e83847048ebc22563059d388d487bb497f409ccdb9d23954b378230d2f50c10274474419be09549fbe71878759194620daf0c94055de1ed00ef9e24900" },
                { "fy-NL", "32342369e5f589cf3f0033ca189a48338c6a521785f7c66435414f85cf667f7543dd0a6530a2bec551db799a9829d7da1853451e84c6116959afb4f7b931f1a3" },
                { "ga-IE", "b87cc7d1e90606308bba7333080823d691e97083e5e2798e3c04f2db92db5b4fcbbc993bcb389a28bc0c8b6aaec5d39c67f47fae32b53379eca78d4bdbccfb1b" },
                { "gd", "1dadaa822f2f8a6daf365a380d9c3a7809dd5f29c5762cd8e72feb751b503e49be860f43ae3129e778402514bad25303f7f467844d9fc126206ee9cf886cbe4c" },
                { "gl", "f36e776d0a729c0985250652e6b73e7ff712c003e95db78065f6527967fd05da8f53d82b6eadb25067ce77b015eb2e34d92cbb1e5060f3c05cae982cdefb2b57" },
                { "he", "8694cac915926f8fb732e312863e4c0714300fe1a95ae9551824ccaf6bc800931e8e6130f7021fe33706d8899e556613007c7514898f71f9bebec0a910927cd9" },
                { "hr", "d777158b5add0d68c4eae3944e9bccbc6bca4195ea429e36f1162af707fcfaaa532b7d68c01c9c5066577e4924d279bad8fc4a6dec50462b3b9d7f09c96598c3" },
                { "hsb", "2c473f99424fdd613b0ce7e08895924be609b0b1467e023efd2013f5b84e3910aba2c7a3aac645c27378e94d37681c2aa6fa5fb60729a514dcc4f52b88059073" },
                { "hu", "a83738453e0896557a769e28b3785c0b0daf08c388f93eb458b38e23d1803d4a974980706ff345f6c5b2789112a1da0a0edd098f73e0e6f9d54d0374b1234e34" },
                { "hy-AM", "c16c25ea93fa22a5e89b8ba855a6234b3c1592fb8f4d76fd04dca84c395b41e48eed1e2ebe657fe87cd8729b88d7b753a2ba854e3ab30445024949252f62d233" },
                { "id", "cfde8269a8ba4b99acf0910b4e822b7f02298ed75b10f85defbd6effd354aea8bcd71955f5fcaac7f6e7929ffbf1903b855fe99e55bd3dc8126a5e3863771ecb" },
                { "is", "e6823b5fc564a59bc3097e7e429d2f444eef745f41c96bc9aebc91262e68002a5be6c2a27ff05d237b7c70ac5835277861ef96635e00d8f9ff22cf855bf91f98" },
                { "it", "c8956aa5f9549f6cb13541041809209a5045bc7b04f6dcf2907ee2961e63cca2322ca21cc239e2effd87fa839bccf833e95a6a0f26c52759c0442b0f88215208" },
                { "ja", "462bbffabe4a8b7606ec34f292e89b278ad7b315f08fecafc5cfa4f7df84748060cb9f50c57f4ef66a43e694f4f2b85b2eca2d9326bcc714f8b48547bdb94dd2" },
                { "ka", "ca72e1c6507899fd4bb83563f50ec7cacd51baeabeedc7288b5be53c97f5e78431887f8d48906b890d6133defad5df1ddf3135784f65e000f68a2a655d1e83a9" },
                { "kab", "f078f1b3af5ba176c7ee16b608f06fdf5293db379144d8969a4b547a50f5d8a5760b08bd65e4c93860abf7d89c078bfd699386caf82e9a4f5d98c91bf95381a0" },
                { "kk", "8a183a4e46651a0e46c773b623024a6b4255746e150da9c29add40df9444f378002ca740fc0673a7a00eced66e6bff23533909ca5c22f009a32f028ededed36a" },
                { "ko", "dcf641fb963cc999682aedc938f5395b87fb6cfefe77f6053510382fa7d6bfad56bbaa175908e044e684ef433acbbd660ba967d95a95a7bbc790486824e69e63" },
                { "lt", "73e29c93eb988063c9f0ae2ca2980c429d5d1a2e464eeb5e0d06e845a7a2bd0ce40db6d21b87b9feb36903256a6f0061a0b5d0555ee965df17e6562bf059bbb4" },
                { "lv", "2110189794b447931e4c2d3eeb4b51148da699be4b398e9557eac3d6ca46ab185eb8b0b93f858a4f52655af555658948ec7aaf479a7ac3f02818f0e51dbd4718" },
                { "ms", "941b4fcb851d10e5d11885657728fb7000a761e19eec1c03df592f99c2509464549e6e433b9e17aaac710f201ab585ee5fb39134d5bd33178fc516a55599c301" },
                { "nb-NO", "952ad10afd154943392bf5dffb743b2da57296a65127d52769964e362cfd76c80e0b6cc4650788ed54a5441d3254821a4b39cddb43fa0010cc58a3ed10734b51" },
                { "nl", "8d5fdc266a435aa96c39050854ca27292ae765fc7efe18df3ef4ca57a55fa6387faa59dd305c17d6b25d95b3a1202cf5de512564755197080ce0c73e864a47ac" },
                { "nn-NO", "aa831822eb458a3a5a2c2603512af6c00be3ea0fcf02252254f966dd2d4e2c29dde8ed9ba2be8178e040e4599d2a16de779a64d3673f4da8897f1512a0c9b39d" },
                { "pa-IN", "bf38c3b210644777ff4828df6b71845cba2aef4a4a83708dbee2291fe5c7f581a9ff25bf253c89c9b632fc2cfdb977aff41ed2c914ff6e4e0b8f0f53853719ca" },
                { "pl", "f0784662ffb6108931790bde9afd644247d9fc11a0413fb2647a660549e42f56ac205249b250502b3c8f8ce4def1c85d134cb60b20faf51e3f7d5ea3ac7c3cf7" },
                { "pt-BR", "41979d194b2cc9f350c5196c05b20731fceb5c2bd2ec95a8f11e1933c6a853d502cd076a09ac92cf43417a244ab126070cbd16bc91272fba88c96cd9ac238289" },
                { "pt-PT", "74e528ef6bb674121e26442a71761afb6791311b0e0194c221b6b93d390e188f1234f573f6fa0e164511534f0a770cccad4f0210534811e4c9b9b8d4093697bc" },
                { "rm", "6904b374edcd0545cf1b17dc5057705db5b4a091805a5a23ede3bdf4081898710ba1eb2fa78362b584b80665bc3de009ac1ab32a3413db1a2a8706d82cf7ff4d" },
                { "ro", "f2490ddf1abe815a2b13e1f3406f9a115cf9a19d1423745912265548e9b9c5fa2473a399b6c35e8df52212e928d400e03429b79d4434050cf08e3a7011151bfb" },
                { "ru", "6aff9bd0388b59a15d22c1e0683a313e1f8a9dfa25118c7b018441f5c9d730be7ca0e0fd2832c3041c3723217dd47262a341a0d1a67f52e53e302d3d166f2aea" },
                { "sk", "d8948af51d2aeb25dc8412cde385610188c48008084a752d7a56643a354fcb55f3099f4532dbcc802513920aca14ad3431177b49c20e7ceb98fd6e24664479b9" },
                { "sl", "7cc90fb2cb4fb3a5d12413d34d183700a77e216a22f3dceb14ed34cd0febb91e323a1cb1d2e78aff1ea24785c40b16d77d1fde69245e09be3b3e323ab6b7aac7" },
                { "sq", "d8cb1f865495a429826e6d5323d85ac524c2b8e2b6b3e2fe6dc6679f45f2a921517fa123569df9beb765cf5518b57fc26f4f1d1231b9bed98d10bdb2c121bcca" },
                { "sr", "ee319d967e21e1dd5f356993b71da6ca6a00cce4b065af1de90980250dda95227954b8c911ee7e4181b296fdfe16b9f3b360adc780ded306d0520b8f8fa4148e" },
                { "sv-SE", "2a283755ea35f4e14535bbe111d20291308c554ff2bca639c4928a328da05910159968b410b4e95ff52632225ace4a1b9b71e5488786fc30c73860267aea980c" },
                { "th", "3569aa205e2e940654f97a117f58a0a7a3ade8ea5606bd9eaa413430abcf0df6d5a9c8df8bd7507316d7a26bf9fa37cfcb43dec07baef23333af3b15c8179c4a" },
                { "tr", "c94be1aba72aa364b8812dc0fe29317192ca424ca77d437d81d095d0a9f2ddb50a588ddce288f58684b868ff73e07fc72a962c258e619affbf1d21d4fe6b400b" },
                { "uk", "537fe054901a59bdb11630b99257d8b530a365122fd6ae6957bc12f7bcd62ec1b2caf48f10155690bf39b4508e0c992464549100ef5fabfe6169ce3740643b42" },
                { "uz", "eaac424a259bd760008112f8654b3f2c8928cc750f7caad88ca8df7b1cf7cdc2fdae1612d355185bbf5ed98beb023d602743429c092d3d48c7fbd4c3811a982d" },
                { "vi", "14c2898f6d8d6bb413fb2325aacae649e444700a8df5ea1b9eb90d16da27f6cca85e7134cc72454dc9ee214c9ab966962f7ea00d4f5120d1dbf81f944ad8841d" },
                { "zh-CN", "d41cc234abbb131b4b02fe673e3c68c6eba70ed409ebc76a24dff6e65d4999784dfa405258f82ebdf2e86cd10079d3f36da380543277fa927edce56f79dd95d3" },
                { "zh-TW", "a13babe9d125c6e72a625e8cec384901f7f98fc32a5af4a74ee15bee3fcf9415f9b5f804992bb7c917d06db4080a562169898d224fce03f3911700eaccdd475a" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/102.4.2/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "4ca74c2da922ca15fa7ceb174c3d4ec52f99250fab00bf7dbae3fa7ba307c4bb1b609d159b81dea79914c2f926288ce6a705609401694f9fd0f9bde0fd1e519b" },
                { "ar", "40e0189034e90cbd5d62478a697affe4effe8963015ed74e4bf9b5599d52dc671dce35950673e7a66dafb559bdbac115283166a8640b7d8cfabc285ed26f8d04" },
                { "ast", "bda2fd6aacbbdc02ddf861952174b979ce75d5acbc16f5870fb6500e3aaee4ef59245c98ff37778fb224c3c538e4ec5aa7279c6fab01544620ef02ef3624ba96" },
                { "be", "10edcc3fe7fee4090640ba7164b46725445d02ffa9b98ff347d9664954b29d03154b700a0ea70f3fabc617013116cc5b9d733497021b7af2aa11ad4fe9f33622" },
                { "bg", "458d47dcd170ba38a8cc07ec02de81287830624d4ecc73e1c20bfdcc47acc674dae87ebe7a682865c7bfb05667e5932da487cba23de36ba7ee22fdc73a59da0d" },
                { "br", "4084df49e3a94320da9c41637b2d34873070ec218817ee06252e23f7ea456eab62f378ac89944b4efe988cbb6c117cfb6b8cce1cea0b5407099064bc39935b58" },
                { "ca", "bd0106dc44f8ce37864ad880948ea25ace0999389aa45c9022c37773b05ab5a9d4b60a434434383a1f85e14b135d4b6c30e1e50909cf14e5b2107e92f75a5956" },
                { "cak", "46dafb8ebe2b67103849c68c96feb492725f989fc8ec23005117bf865e28c0bcf8026e216bb1517ef8443467024f71f148853f4c8080d9751de6cfd0134c5210" },
                { "cs", "c6e77940e2ff366a8a2116ab353af6ed17bacb2ddef2454d1986fedff83f2237f5bd03090d75f74f1944ad5dc35b9552d12622fd4c8ea93f9e9261172ab9ab8e" },
                { "cy", "234b90b855edd32553a0976aec9aa70ffbb0ee14357b97f78837e496967fa8d1b81e287e9c1969b8a58f0df6d2ded3a9fde7ec0567a38c0a53ec6f9c0668fbb2" },
                { "da", "b779076d33ed09f6665b5cd86e9dbedf6eabf96d8c7134f9d5cb353f3034f7fb9fcce57be5f4773ca9bf052524bb5139a28cb645760c1790af5fb96d2eb39569" },
                { "de", "754dc05ec57bdd5afe0f26af392a0957ad8c9772464f96560eca4d730d5cba5d5d2ff49bda126b66c7d7184d6fd6c7620a4dd47bd9e50d90282a1bae1f975eb4" },
                { "dsb", "a44cc10fc14352847883ab0cb38f6d1409de735f44b0618d37f08de06e85e9ba406f025181287f9a3bd63b468c73c3c6bb303eefd8aa2ba93179fa97e5b6e6c8" },
                { "el", "df45709e98dc8ac1cfdc4cfd01ae5b6f20bc63a052fe6e6cbb3b01f4b295a08b845b166ee8f5dafca115f2def4cf965c094895510f237652c166fae372c50818" },
                { "en-CA", "e7ebda805a995d7bc6f4014941cb17b8d3dd76398b6fbd5f7b8da8619f7d79cb1daf389031b0c1b6c9450022b785d0ed1b196127481e2c458e26fab088eb54be" },
                { "en-GB", "b52493cfbb642d310471ea7279ebda0e228b393bc76126b97425624b1cafd488cc303ae7dd34ad7830d6685d00944a612b537096c275e26710bfc6afd72ee885" },
                { "en-US", "9ee0f60e1080e2fb72aa6d737d4ad7d777034a2bd7a62e4a4fa2c64fd4cd0c66a4bcb3f8b480f719241d62487e0c0d69205759ee592906a3d8c7b70845f8c97c" },
                { "es-AR", "93882258499e58efcc8de0b22f20d6e5446bba9f9a2f3489fc9343e7bde1d885158f866240c5ce8c40f77b69cbe45806220c07a267280730180a81fe28c2df4c" },
                { "es-ES", "12030338b68752ce8307546ca843002a75e3d98c09243f01bb861ffd48805f02a35e17576d5f1a57788dc4a16201bb5c48a7d201238bca882f99228f7292a3b3" },
                { "es-MX", "80f697f75ae56d473a2645c2d48b8d835997788ccf5436ddd0b8c2d02b865d7ddeffd9cf95aa63981c0e72e915b38ce5f1ead7aedaac053554da9825b7cd96e5" },
                { "et", "0bcd65c3e88cc17f263c6c160ffb99591343afb281d38f634a1cdef51b34e9a5f37368689a836a94339ab3a77620df2d4ebe67061ca7cdcbaaf9d2a166865bd8" },
                { "eu", "65206e366f7feb17c37acb3c51ef9934109209e68c08f1005b70bcd77293c07b65fb4c63dc181927da6ec372a8b40020c8fd3ce780ce53fa4e3858c08eec0979" },
                { "fi", "1ca0085ea1e70a4aaf4e53c2f5fe2662bf3a50476d8af9585ae20f1ffa2206e0137d64858e94e6878bdfbe4fb6015f821504be59780cd3e863049b413e91e13d" },
                { "fr", "21182fddb79e5f85ee6d8d6e59acd3699ab725b5e5445f84fc823ecba906b70c58279a97b3cd37542b0a8e7fbe8da62133017119170365b2161b606d6e8c8642" },
                { "fy-NL", "9b76ae39105846f540a3e422c1f37eab5422ab28e17402eb2a8d8b59c0d1f2e5075466358ad028795c4d360ded7f4a2d366b906bc1a24af16a4d04af4e69d69f" },
                { "ga-IE", "bd345ae275956c8932598953e156b251840443d89828a68a4d90b77ce2375b275cdd47eded7b0e90fc734c74a5ecf4b18cc2fc1c74c330860ceac857a17d8414" },
                { "gd", "b552c911b564115bc6cd361e1347d2272bd1ac024d90b009b7d69d6b341cae7872dfdc92bdbe6f71fdbbae10a283d8b2144292df125579e5adfae369caf3ff9e" },
                { "gl", "4ce80567605477a6a7553b5ccf86f6f6314461a25f65851ddae9fd0951f87908f6b2d16b30b09d82c45d45e18f7f438a99cac59b74159d0414edd02ced05fddc" },
                { "he", "0a031c39c9db13096ec0ed9497919f4ffe930de9e6373e9d4214e0fbfffa61f509f45761a69332cfc30aedef8afcff2dcc9f46aa792aa7a003408a3a3ecefe45" },
                { "hr", "e2c3db82b75a21d52e1b441ddbcfa48648639574836bdfd33487cc757b82940ea7a2c919f333cdfff3f2183578520678dde10709b50a3579ad3f3fe82d031593" },
                { "hsb", "6022ebac869100ba0712ac7857cd3fdb1edca421f5001bbdb280229572db007d6e6e9a21277f1c7ac967e7fd43117e732e847827e31f2276f6b06b11e38b3f37" },
                { "hu", "40c33c7291824d4147bacbbc3a6344e59667f64ab72d45a8f5a904cc664bdd47d904a7ded110f405a33af6ac7dbf5711d23795c4a6ed167285bbb9f84d9a4dcd" },
                { "hy-AM", "97dc4348977f6fba73ae95717d8109931e8daaf46b337a30267c35570e1c5477614221730ad38d324eecb58d0458767487fecdcc2400cc5c132a7cef6199414d" },
                { "id", "c77dda1c9aa1e84602946c8de10d6cdc11dfa36bcdd481e0b61d907921f23f28eee5946607807da62c89a450ae9fbd0de82991dc2fe66b14a98cf5e28c89d0e5" },
                { "is", "994ddbbd1508262aeab9a05e56d2353223fda1e28a2732238eaeeba6e65ea1d1c1db3d90763f2f4409e830f2b8a45cd800b441f5bbb657f123b2584a18155d28" },
                { "it", "b26647f7ea79b02ac94283d34829ffd6d9a9e73418e07759a6be4603b68f5a52497eacbd1ae634e49db4dfc8814e871512be57e20c9b15704e99c193a9254977" },
                { "ja", "d7480d858c01a492dd20bb9b622673fbe07ac55e92a81293526ba905192086f4e2e9874c57b9e8ee9bc7b7352572c0c4fb46922187167521a5f7982b9c2116a5" },
                { "ka", "ca071def36de5b4ddf3a2f309364e92209d4a628459410e975dd6aa2721c0959b04b76563fd70e5cb64983fad46bb80f36b8aeb404c3541e0a60dac0e4839728" },
                { "kab", "c1c177cacb352609a043b3b6d6ab758a63301939cd102ecccb67d57f8a5dcf356ee20d66ca0c9e8934a00371ebf103915b1ecb9b1473b821e81fb63ee99bec74" },
                { "kk", "019d85e8899878da043abf192f2dc1cba3b51725aa5786b9bf581994603d36718241e163f79440b5d1a564f121010a6a4c2a8f1530501f1339f5650275394c17" },
                { "ko", "b5731b48bfda2b4eee9c54408cb1d91cad3e31f6fd9706db1a142168989cc2def3f176ac0285911248a7302e27dcc60d13694df31a551f03f99fb3b977d09e12" },
                { "lt", "8cf2020d40e2b0e85db085c115059ab6bfb4f33eb72681436f5979c7c5888c9566028e12a38e7e3ca1e322d20b630be8ca953d765450b6af6a40b727a7e4f61e" },
                { "lv", "26ade5697a776593eef0f85a76e3bcb7f7746df7f1401953e8b405e053e2a0060e3209905be92ca5e9c982ba2f0086b072f57338e18fb2b6ff36de0631be385b" },
                { "ms", "903259027c9ab6a48c89dc5d5c0e59deebbd39a253717cb5170a7b1d8f4309c5c63d1f79841765b844faf14b506b79cf9d81a4e4085824da3768004e42ace4cf" },
                { "nb-NO", "7b6bb638dc34af2b8866c6cadcafb9970151f29fe82b689beb71413321be9666ea7b5aff7c9d9921fde3e3cc0cf2e7df6d83f1e4dfa63856e721aa983b361ab1" },
                { "nl", "51f6aa9cc3f9c65aa77fab90eac341db9bebfdb42c59cd150331f65cd381984d8a9792d030c9b5f0c5e06e543d126a983aadb88c158a94f8932170ce7a15dc58" },
                { "nn-NO", "6159415a1ea614f6423e1c1a73ddd6fa1a7c20a1842e299f7db2d280b19e93fcb2184fe5b6925025f313f0a68892586b2a3f804d810a345de674988b64819f24" },
                { "pa-IN", "b9f2c44ba5c588c8f3b0df603967ee8ea557bff3fc83a8a01729cb86138606c2da08755047281c763d2179be1edab21ddcb8ce0dc86d6b747c021d00f404183b" },
                { "pl", "c0f1b72a2dcec1d52597d1c34a09de1cd4bb60ab84ec2eee105ea2fadf08840e6147fe48d9faeb8ee4739b857fd1d74f94b56ce13db2afbd762a0a65f8ff05c0" },
                { "pt-BR", "9a45f9d101ec8fc590afb6eca324ae39c6acce58b5c8a96962395a4929f73a542c4c36bda9f007af0ac99b4949a4fdc588e88b27f52069276416c47956c9f265" },
                { "pt-PT", "0f22d5db9714a64099bad3f65d8141ce2478f3fb0b92cd9dc1f328116cd3f88d868b927f27c6591e4b01c32ff05feccb8ecc1140bb5eab7c496fdf0298ab2428" },
                { "rm", "06f729d80761a9f92fd9dc40b6261aff8519e56cb9f8909ab56ca4ec88681a888ece3b2efd62c1f1aed17b3e4a13ed633129b31b7604d4c343f9261ec48175d9" },
                { "ro", "b33b36f5cc05f82831939793869838bcbe901237974c046871604aa41f0d115cca4ee53e13159ae088886afeb16273b7a283f0b4abe2692f234cb102834fe1ad" },
                { "ru", "19d0c4e77872bf736de0fcc2e9fddf8cfccd442fcfd17e7f47965f12b5bbe77caf4dd9de7961b71119b17d4ebf6ff4725695855edf725369b24a54c76218ffea" },
                { "sk", "295b867e9ed7d494f7a84ed18bfa8271ebf7cc7e34ee3a6b1acc9ad3c72881acdefd8bda1f538f535493447a9e7e963d0f6ecdbcd5e63113ad7fc02704f4b506" },
                { "sl", "4fe41bc67744555034bd616ad1e0f35713f174c95578d41843b9f43b00ee672e6ed7e89e824d2d82154ce61423eb855db4c8b73ae26a39f1be6ba7db9ae74757" },
                { "sq", "3bfdb09c39db967d5992a66edafa747fcd48d3e69c7063b7f0953fa655164f4c6b2ec83de72899dbfde6b54146b7188e8ba5ad9faf16d8212ba452e87a432a20" },
                { "sr", "7d7ba7f88bc5ba59a1e66efccf6461b650c5071cd49c9f653d2f50007c9efaf2f6d03e0960edecb617b8dd5d00e8a2d07991d6f2a13f1783aeb7374fa9fa61b4" },
                { "sv-SE", "c2308df25ce387dfeb643ff88a99159b051f974e65d08f1832eda78bf6c822fd5980d239a2097454ebdd326004537e6f4f096d5e4193d6d8fc0b59e957b05b17" },
                { "th", "7816550c1ee750fded71e5f49070c8aec6b7ea4e2b9942b8b71ccc90a02ddc64ddb6cc34cee9e276734f4aca0130d9fa0ea9a2b41f41386335eba5c1912d1a11" },
                { "tr", "7d2f7780151cf730b72c29d43705612dcf69051b5c888304248fa864d8f8f91727d84b00750075047adb58a481d5d3bed069ad16329681a1a8868b084e1071b9" },
                { "uk", "dfce43523c1160d0d7f579fa580e7de41d6b5732b1a3a5e4d00b196ab46e954dea9486bf270cc1c85c1f0eb3c12fbd250dea8d428291c35b018fad2ee4328d1d" },
                { "uz", "f8c192dceafb9512914e112a7302bcd1faab3624088387602326fb36f36e587bd8635251643cc9bc9728fcf97758d39f367b2f223f4be3b3fc42b6ef4cc1947c" },
                { "vi", "525bab329c913ed0de0c9caffd3ec39715d123543b37d914890e5493e8f0034a1c880dd4deac37aacd75b97ff09338113cf19d4a01886558e2182023a5b20b73" },
                { "zh-CN", "bcd0c56c5ea31c2fb11423c785a8be03621bb93fc453f561458bd3febbdd04ee7bd7d1871c8174ee1a4dfd5c988a0a3bb1e4546b320c02f834c6148c05b90970" },
                { "zh-TW", "cb2617dbce7ab505720754ee83bea38f83ff129760ef32961a5702481dc98b7c0419836a60d859b07f2acca8d08f48226a8cfba761a999bcf3b2e47fbab16b27" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "102.4.2";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                sha512SumsContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                return null;
            }
            // look for line with the correct language code and version
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value[..128],
                matchChecksum64Bit.Value[..128]
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;
    } // class
} // namespace
