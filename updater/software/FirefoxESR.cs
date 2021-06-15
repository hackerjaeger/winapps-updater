﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

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
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox Extended Support Release
    /// </summary>
    public class FirefoxESR : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxESR class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxESR).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox ESR software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxESR(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/78.11.0esr/SHA512SUMS
            return new Dictionary<string, string>(95)
            {
                { "ach", "377af2ba63f28785551da779b3974ed49e5e72cf414837f95da615fffee3fad4b4e766b49e5503c2529a1f8505d94127d5d119432765bdf791430a35fd6b9279" },
                { "af", "d31cfa85e795013fe612d44f48f2b0b2b3a1b1b548c54eb7ed252acd9ad7d1d1dbbc8529854753888edc50142ad9ba590a7fb0682a4283d225768b74f6fe4204" },
                { "an", "9fd25228ff9f16477bc874f95f9aac15c2dae2a1c28e8767416ed6cda14b59caa36800a9ddf8bd39b2984189cd6b95df4cbaaa7be518f44fab411744e88bb5c6" },
                { "ar", "8e54ca9583166bcdd5b41abb7fdaa554910b0af52306e43fe9d042fb00e79d1a4272ad5aad7e1cd24512585c5e6de74aac2974d59a4442708fa3730ccf4bdba0" },
                { "ast", "065e35e980231ab1907dbc5e1be386e54a24f8585e708ef746031e14cdcfd5c24765aa191cf781a18ae26e4c39b92cd880729c5d1af99282565a69a3a759e617" },
                { "az", "2614ad7ee7ef99e9b9ce00a3f44a9860001de54c3fba5c916b505a302e97f0d52820c7b26e77662292d7dcca49bd565f7e8b7e6fd14ed5732ac2bbcae91ae8e4" },
                { "be", "970eb849e617c1b5b0e7fdcd7b73e61b1fe95e00322787bb54aa4bfb05a2580a95ad846ba4d5f9d557abd201c3859d7f95bc391810758c55d5eca1ffd505584b" },
                { "bg", "406733a38d1b10af6cc2aee0e50f7a136fa4c4e3e2f7857890e0abe92d073abf83a7f00a371e643ac33644d0a7178f5910ab898c194dda4c91e0e41aed0b375f" },
                { "bn", "015e5510008656dd00158d5e52739e830d68b2eb80d0432d794a27fe6cb0e83b52cba03f54f7dea711ec63abab0403ab62887ec36ed5e318059aa7382834be6b" },
                { "br", "22a009d5f7861f0a3dd18ab3ff774517182b911886394c354ceb5bf2d5395eaff4b03a5112048e1fa2f8a5764d7f33a8224be75c2e6a18f63bd5c2b46aac0868" },
                { "bs", "dec40b855db5ddd96ffa1489cadf1b56adbbb0021eb52c2d5b43fef8c21ae8cd77b4ba4588b3e7a870ae1fdc8a04876d9cc625ecadc4c6c407e510e45da5dd7b" },
                { "ca", "b6b6aa7e50a769fb5a8ff022cc432dba223729b0a967ad1ab5fdf7c8f3fdf87c327d00b2c82d2c8bc168fd76f7a3728c2b6bafa65d119ff3bd3336419b6e97ee" },
                { "cak", "3685bfc8cbe6c0bb7c7a6aa33708fae4eb6a16b5a5a6e55ecb336f2c0cc981138e0f51f584559b5bad17d74bfbd5a98b9d062a32ef9c317958a23c1a4f3f51a8" },
                { "cs", "6e4d2c2f1a715a6375ddb947049361c2f778eae1e9c6c46efc39f34e059bbf99d1dc8e9640a11546274ff5414c2c9c827b6f35de7086087dc433f5f63d4a1322" },
                { "cy", "d8e75f6088eea275305a2c4ae650462103de7e8f9e0582047585dc7023986e35a594695057b7f9304182364ea106907377b13a85650ae9bca76ec83e3d2deeef" },
                { "da", "0f184030eb1304e55296682fc99790d012c02b01d093891f9542f493b4c29b8eff872742605f64d10b03c01efc063d3afd56a4d7e3b2fad1d691c1abc3f508b2" },
                { "de", "49da637fb100e04d64fac402299b5358b89e6fb10aac4ec841e3a20d0d8b3f8efe419021923088d206b1adcafde0730f0d61653994d76cd3cb2220672efa8ff7" },
                { "dsb", "9b058646f420d0b49177a8a20d7c2745b92975309cfdb9c9686ab882b02023eedabf6e311b5f701f80cb69c0f27b374fec2e3540bffc3a4ea1f5aa851e2665b9" },
                { "el", "3963fed54c1e1def405251149aecbc9a6c3bea92ac52a262ae6ac0a846ee2e34b691742dcd9601809294763100fcce91253501a17301b3673f70e05b40cbba86" },
                { "en-CA", "9ce07beea97b78f9d4fe49da46c2022aba1e1502c5c6f43397c41159f774ea5f5ac48a8c6f17927935bd140ec3c5ffde88d7513c4957da459cc77fecc2aceffe" },
                { "en-GB", "f909f243f595ee226717ca1145362c7ea594305bec4b8fc96f2c970692f3bee667e6f66b6174a0a0de0242692f46186570ff2b0f5858f62dfbecc2bffa4f3c29" },
                { "en-US", "60cf02ff2c0d276c835904c17697e206ee4ca45be7a3c7f575f2568a4f9209626cf9fea5b311a0e22e37f0a2bb60c296a8140a9e4ca5a5259039816e8c0c9a28" },
                { "eo", "e430c8cfb93e8afd6cae5bf1f70cb9e1a2f9dc776163ca0a4bd60232e18690f1d3135b6f0f0652f8330215af5c8639951a661d1b7cde527a1e0c13d0e0f57004" },
                { "es-AR", "d95f1817c53dd8e9430f500c3a0dc7899e1e962be283385bf94644fa5656746d64650b9391bfab5cfb65b6524b97ffca94c31410e5b98eaa27892d07e096b4cc" },
                { "es-CL", "7bb5ea13bae0525dd3135e9c908eca362740d065a4d7e578ceeb4c7a822ed66c606300e8c10f9dfe947ba1abe0c78a3ca58f14b97739045bd94dc42f919acd5c" },
                { "es-ES", "e82687af70b64afb98af7e9fdf356fa870e25f990a38d36ef34abe42bbe07e434ac4a3a3c9f54cff7491608f5b139a4081e7549f5c759a26ec032f45c5201856" },
                { "es-MX", "e66168f36af3c01dd23203bb86627f8b68ad8b891558a1e4f5301fe5a1c2350913892f99e2ffed9c6e13f7999de7e5c9dcbd10a2743ab175c3eba086a160af24" },
                { "et", "6251c4b26f941efc7693a94ac84761bbe4ceec6a7b35379408fccba0ffac0edf75df8a2199dbc30b861cc1ff1b950f1afbec5bb5e1bde4836362d30d313f4dfe" },
                { "eu", "15f81b7b3bfacf58a6c6eec2fb8e47ce55bda9da3462ef5ff2c48253b788dd35728558ecd7ac974dc7aa0733e32e7aaac3ec44f7b8088fd96d20f477c085e58a" },
                { "fa", "4069ce752b27cf692a2a0aba476ebff0315c5829a58aadf1fd99006cda98aad55b3744da375723d3ef821e72882885962c5711f8b4792f85b4fb836032779203" },
                { "ff", "4e1b1eb71b479c60ea492fea2183b6c4f44ea53c11efbd4a76ccdcd4d429088f13c0f12d55bd8624d295f5ae5e4b959164197baa18e9977b75422168e42c9647" },
                { "fi", "8ae3a2ac92e3f09a59f07eba645e4715cc88e2bea13bbaca09b5cd1e774ff022705040fb7cce055059c816724531642c06c2fad0f42a34b2a42f0f957b49b1b8" },
                { "fr", "0659a65ab20bdcb9fe10e4f298be0f0ac4967e5a797c70051af453042156c140507ffc3a33bfae142c1be4566a3fa697518befee11bdb94ec7ed6bc99f1972d6" },
                { "fy-NL", "12e4f6b547ae64be8e036ef1cc2ab081b58e06872afa555a1cabb7ea660213f3ed00c699ba5310201d58c0824a1f210c7a17ff04a36993d0bf7a6b969894283d" },
                { "ga-IE", "8bd4b08e73924dba4da0a2f32705a18627a8c46377761c48d79cf09a16da946bf7760952cde04c1f3019d8e39872c96b67905432fde95080cada93c4e99f02d6" },
                { "gd", "071268cd4cb597ba6fa5e89e8ae848be9058ad2a18b2df7c944bfe826056e6a1f4347ecc465e22b67a9788b1ca58981909dcafd2e6cb5c852592208ac0bf3952" },
                { "gl", "92461e190f5c61bfe12393148b2f0806ba56832e5af394da140177d7fdfbd268a6ef173f8bda530cd0c72593d939a14e11059b6b7d196ae47634b995168547be" },
                { "gn", "9afc3cb58935acd5d248d2bf0dadd381882f93f24ffd53b83d9419e66fd5431640b33f21ef36633bd425da4ed4fab0fdb2a40b8cf3e2545fccec1001cd5e4194" },
                { "gu-IN", "aba628a7161dd2eee41578a2d800640eb2fdf290532e85fe6ae71a9acebc80ab00097fc102845bd2bac4101b37fb76c192f40d7434c33f9c900f5f31b96d051e" },
                { "he", "1f93357fe2cc8e109204c56bbf489be10b995fe2fa820133097626cc34694c5099d477ac38e239222eaefacc512114b4a92b9e6cf0868024d4e1f7aa9651b03d" },
                { "hi-IN", "f9738b9bbf6d1ee7f4a529601d8b75fefcd61510ca2c004d13222ae7e0694c9c9524f55be471f360ab07ef30e2f171942c6ccfd09a5576e1604472d048ea4a26" },
                { "hr", "69214fefa2040f061c9bfa5876da8026473b785d061cfd8c0b45f854c415c0ac639abfcc6088fe3eb4c19c79cdbdf5142ff1db4ebf49602e066075753fded1a6" },
                { "hsb", "d4190ef9311e47c01398d76455949adbf5af1ee7cea9d8ea5c471767862b0f156f8b1164c3335fd3b108e70934ad74ec4679894670606ab9c1896882bcdc9686" },
                { "hu", "7c8eeaec66f8f5c053ba09a5f6dee887a92198d5ea8db4d82674e5abc1a53815c990c8cf15509efac0408b4c9326e038efdfeb5b1b98121cd0cca322ca38c6c4" },
                { "hy-AM", "dfacfb15cb628ec98d3d7e0e01829284b052f70c3619569f3c7354d4b195fb1c05e010340996b51c2d5d26bd72a26fcf378cbd29ba2ac4ace6243cae794ef274" },
                { "ia", "861ad3272c874ff66178665fe0ac41b0ec06fc192b79baba5a81320b0b5fa29c585041bbbe396f58b37c2dc718053b6fa0ec63830a2189821a59865cad1cb9b2" },
                { "id", "2a837c1db7819255139dbe3141c417c8093a22e6447b720d20bffc434da7ee624f9b88939284cfadc6481383089d9f54001e60a7daa10d392dd85308f3b7dea3" },
                { "is", "aeb4e5a8d0486c17e9a73eea2f2aa2fd30b31150b4fddf34cba23572781c466b5cc938b87715d83c498dc0caf8766057cf52f89a20f727b8292b6554aa74bb31" },
                { "it", "153610ab22b1a549d10fc3974765bd7a0843f8e9e912027d10ff4f44a41f7b034bc191158b670053d95755777e0aaa966c71da4954a13d0a1b2eb72ee52b32cc" },
                { "ja", "0230fd41b4d0bbb58747f744d84aeac3dca32759f5b37a75756fc75737e39949368caa735059b8460bb99e0a1a071012b4b4f88643eb02b725ac9c873964215d" },
                { "ka", "a867eb01b2801445a8a5cb14812876db4aa12fa79124d1031e88367cc8287ca572892888f95be0a5cdde7c0db103a88da42c93d729baf29c59a730b4b818b7e2" },
                { "kab", "dfcba2104ab5a0da7ba7221c0ad2633a01bfb60c8df1c718d76c22c45475512b6981cfc31814bdf734bab7cf1fa024187c88b11c79f956d9b5490e384b26d84d" },
                { "kk", "3b9812cf07b13fe4aeab3c9aafd8c1d201b3b587d716620e708823f7fbcf79db31b63aba10ca224c76f22559672b512caffb65e1eab579033bd244110a9b020a" },
                { "km", "3b3f4e812dfbc98c7e92bbf88da49ad54e9d8e401e77efc6a18a0397ea5a6e7aa1d4482762389f6498bf7b93597fa216dd5dad6afd0af0e87a34a58c5593fc78" },
                { "kn", "6863605dabb84db029a11345dc541b0385de113e2d2b44b80c7a70f0407d5ed190afd77ddc63d8dbf05cadc3f0b08977a7c42e75f47cd4c604e7d67f82fdb627" },
                { "ko", "7fb11f32929246a4e7eac82acd0c8d6e7b6bcb8a3ea52566e2d853193d28d3f490a346f251ddd779ed4eef8072b039c0c6bc8dedbd01801c56414f9c61c743b7" },
                { "lij", "eed309c963f3d77c29ff3b82204c1b3e87a881bc5fa42cc0a71547dd8a0378f8f329a9b55ce6e329a83c1ae1544ab1720da3112bba939e831d308f0c6fe29335" },
                { "lt", "ce4f709bd27cb52f2e60b73c4ecb4af152755689655b86c62818123bfda648617b7d869fe8aa505651ad7f52fea6678c6d1025327df2aa6e87d4a9f764ff4390" },
                { "lv", "53f1e58154e1ada4616128beac8b16550a43b5397f2463d468f1ac17c59adb4ed6f381f1ad0616440cdb01add69206b955d41a3a2fcba288507ad7bfea68fb18" },
                { "mk", "41969b49246ecece9d39a6e7a073c8d9b738d90173b7a9f206faefae55f7da654eb448181a75923ee6adbceb8a6ec7b33a36676f13bc2d7771b93a67b743c4cb" },
                { "mr", "a41d4e3810a15af9639c1888ab09928da23e02bedc0a9c03955b4499824e2bbbcbba87af67661f720f4066a407fb0f3b027f5ceb2a8a98b06af1f75d7dd57763" },
                { "ms", "eb8875c48b0790eead1d19d099e3d0e8d126be16fd2e002ad0523eed4749d04220d9634ed9ad492efa3b1bef7ba56dd6bd6bc053139166aa8d0a42c07fe8bb21" },
                { "my", "d1ba7a9494c7777479fa0943a178aab9f8206ede258581315ee98318bffad44d09e06d56943fe40da2b15dd0145c4804044e9d4adb23fee8176eae4044946e3b" },
                { "nb-NO", "4fde865417a0bd64d752eda5ade23dfe0671faf62280631d2065825407eb559f1184b8c1415f3b286101b1d984921b483001d94a5ba16e2d8911a0806d2c51df" },
                { "ne-NP", "257619aa17fe442640e1e83c5d3d00b7f8f09d913d3f4d0101a96431440264f9208889bea8dfec44e11660cc51730ad200c78801170ad0e7514971c557d20933" },
                { "nl", "f8a120d856b3325177d1f660eebe2635b4e76121e776a828c95d3347acddea84fd1e0a7cfd69417da501bfbbb7775355b6000dcd47790aa990c9afc449d64dea" },
                { "nn-NO", "83e24bda791884998a38d50fcc049f5ab90c1722958db7e3b6bb0e672a96455313752e37446366c1c0e2095781844faa8c9c9059d020d2962ea149f20e7ec307" },
                { "oc", "fc9bfa90545662006d0ea7adffbab3ae3699f05a0b441af480d8c96d70714dd47219c38638ed83218a9013958d28559f88664771cb28c426ad9641888935d3f6" },
                { "pa-IN", "626dd88a5dca2024bbb7dd4d83ac37fb2e9225264c3afb5dc594af0da14172529f07c96ffc370039dde1868d8b4539618dac686897d5390c86bedcc8da9d97ba" },
                { "pl", "248b7255396c1b3508821b1b52c7697f503a1d9ca06f37db217b031b64485327cf322daee5857edf38a092096e957ab417cd44de58706b1560e7f7a8272fede9" },
                { "pt-BR", "b9625a862357a22d08cee1d5871f64427c39847d6312b9bbbc0617d7d1c5e6bec6f62e2019d2c0ffeba25216e082b2a22d36f50620d3e1c98b27e790c3982ed4" },
                { "pt-PT", "d43e0d524f190d106e9fd4bfab47f4ab64ab154c2a1d1d99c552f8428cfa5913a3bb7d78a9f5824d77fbcbe56a12bbdacfb9e1e3027e9ee14ef27bb5eb967785" },
                { "rm", "a7ade393c7dcfc240c9188e8d13d64bbc9ad6eb86dcc36cfe7c2d927cf2ecb28749575405d20fca0b83529fdafc58d373cb3e61b999826dfb7a432a5fd1ccdca" },
                { "ro", "98645c5bee895b511e21bfcae905684205262ae5ea3c3231c6087cce52b1c64e961d8264e107d576dcf5ee4141ff1a1e21f804b13af69d1ecfc9e6f4659bffea" },
                { "ru", "5626fbac285d0f1b1f7d5b1847b6bce7122c9ece0ff4adef13f374cd22ab77b97c02e27ccabf836f65e7f598adfe05914df4f193265b9b622e9d5b073cf8a6e9" },
                { "si", "2b50e387c98a0a71705397eb916b1c78c4407612fa9440aa16962e1be4039efbcc7211a6360d384163643fbdb8c97a9b3ad91ec53e8d57eda0e6d4fc439ed528" },
                { "sk", "fe9583db40aee776b730632dd9b2cfcdd65be1e1c0d189854ee80250a796e0d75852c1dd58885a853bf11f8f0695d7227d06f7051c5cdb0a196b6aa44e62b4dd" },
                { "sl", "9888c5787bececb20f220ba7d979b29cb10ba173205fcae2bccb100bb363431d83192310e0a897ce202f6175d8b20454ffaeb3f3ea3a8c8e67e20336d12cbc41" },
                { "son", "f58f9daaeec8f2c55bd68301f577d0baf2f6a713814c9ceffabb04b74fb06536f263612ab0bcb9b6475822ad1b802a5d71df5ae19838dc63dd59edf03e7120f7" },
                { "sq", "bfdd9894f5fc2c661d06cdbfc3cb5cb1ede83bcd5db2c05b853641fc94002a69a9aad2c43310b98ebcbb4252ea8b0c992a402d6cd842907221721f3c67f3ad11" },
                { "sr", "b88271c301831a291affcb305abf61291640b053c55180be5359d651c54b354403425eab2d1d26b543768af5a1dd958eaa0b90a949a94b0ed2462cd837aa7a3f" },
                { "sv-SE", "67931b70fbfeb511157a640f4e918e125ffbfdb182cbfe4514fcb4ca9c43e7ec88ea418e57b3fd38160b7571a1ab55e8a2c0b5bbd84d49fe44053a1c6599e64c" },
                { "ta", "ac38ec11a7244fb314a2471cbfbf6280c9a1230c57ca0510008ef7effb30d1ad47b86787cf819535eb20192715069fdd76b8cde18cd46770468f75d103222978" },
                { "te", "a20292dcde1753696b1afb270962f29cbba7ec11b32404b5a5a9b3758b350bdbd39457039038a943bd49b7dd9f86d7fc168416d1cb796d51b671a566ba7edc00" },
                { "th", "35b289b769cf210dd530127268f4d2ffba577154afb4b6f7d483c721df4cc41b55fd1412247794a197f7e30136eb72d33c470982730492ea3082c7d573aa23e3" },
                { "tl", "860125b74dac11c99d57ce7a5a028fc87dcd2f0d3d62f936e3ca4da3e117b4ae10fdeebb264c686a8f3c725eaf71ac636efade0845c8ec2132e766cc20d1f957" },
                { "tr", "963ef388b3cca3e65f28e581063c967e4b22e29ce9e42184a216484a43f2fbd8ef0671fe76bedfe3e86d2090a32a11e5a7b16b257442f2dc4a1803af2397832b" },
                { "trs", "11300c0d0a7a8986dc45c3b2e158859195ab2b7a3167af68cfc28ccd26ecd62683da00e33ff03f6fbdd268ec8942ea4b1af34631d30ab9e215796790e1188ac6" },
                { "uk", "464269159ac285907288652a3b0f276778f55620a82e84ca89459f923b044be317515cc8ad4e9cac062e55be60c31eb4d8b2beb3704ffaa0ca226846d567bf5c" },
                { "ur", "565fa1e1d6f4a0356529f519385b9b4f8b7fef67fa444bd0763194a352c72159304ca428476091c26552a429a95ada704d3fa7e9d10f9beb7c6e824ba70328dd" },
                { "uz", "e0c6c479f0d211def32bf1f0f4ad7877f43449d0f50e4871924f29ec91da66bffece4ab7dc02c34a1c49f94cfb14530acdc8777358b12756e977cc3ce2f49275" },
                { "vi", "faaf267be3ef95fc8cf6cbfa746daab5e54f2876ddabb318ac37bdfa8e1494f2a52d417dd5d4a32a87a4d63fe69706f3fa6f73be088643982791ef76ed71d7dc" },
                { "xh", "94c68fa307e66d36a8e7478b53f9d7c2a09569c3590d09d336d1b76516119d185a3c6d836f30f829fcdbbe46c1d5c500735c151feff706cd10dc5821523f8975" },
                { "zh-CN", "534a2fa96cddc3ca43ae59399325a11e2fb54bde5ee91d4c4d719fb1b2422fa4683105612f1f309ef6fe3e48897d236e04ae5b5ebea01e19150118aa83ee068b" },
                { "zh-TW", "61b47b7a3fabf1049f38d0fbe900836fde1480549b58b66e15f71cf4db956d133c19fced6a1bba2ef777758612467a00e3029c5133a96336a85f52e98cfb7f58" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/78.11.0esr/SHA512SUMS
            return new Dictionary<string, string>(95)
            {
                { "ach", "850bb5f74afa84e826051112d16383d0edbf2422e3626402df756ccf97ca77eeb49c969acb631d7ba8805d1c75202104431e39b4ee8de59b4048abf98ac235e5" },
                { "af", "cc03ff2003b4c01dbfe75c3163fcbf0f8db385095204527da8d85bd9c193b437f91cba15f1c832e97eb3c9125173672d2bd80c0e7fcb0bfadfcbc7931333b244" },
                { "an", "c8c58034b9baec43a0b632e406f027c393b6798310a2e10e3711270c32722fddb53354cdd0727b285404b2c73a8cd96e7fc3b87a5c5e8538cbf29c00f4cee28b" },
                { "ar", "62d29147175c79b903e1de56ef4633c1133ebcffbc69abc62f377005a646bfb57d4a0ead66973aa4f8498755a6091c2342485801e15b8cff61577d3a28d35389" },
                { "ast", "ef713d31edaf9a0f5f703a92955e6f14668741f04b21ec7d7c10c1e213d5d53748d8a6c1baec5b7ae33470efc9c9ec882851e66316e35f52d01bc1625d8b9430" },
                { "az", "5b798e05604be3f3a26b7c581a0707aaff03917003bb7ea58c63473c83699b1697afef39f8fbb0f86988a873cd09e3eadfa35d563310fd0df489ecd4391bd7df" },
                { "be", "635c8fec24824a58a4cff48d049b1a267ad628cc05e1aaa2e952fa3f6fde0ce387d768774917a9da0383200654399ff2592064d9bd8919f36d71593c201bd359" },
                { "bg", "a47a7b48c7b1441b37244675c2b7a25213e644a1dbabc9e8bed4524464e44e8880be5fe4cb8e16a58016271da4a95d55d536cbd2cc87576159c1fe7e1e7dfe0a" },
                { "bn", "c676dcd49de68815dac8725835b5180b21484033003ba1d6f3e27b9539bd4604067ca7384976e54f54e0941ab81f7d3bd7e5b2df167961ce68cfe268d42d6a74" },
                { "br", "a34d99e52c0ecf46e27fe589324ddef8e8f42d4b812e64cb4b83ce62d3aa19e0293f040c2ab63246e1721c0bd5abf2fa69ba281d44065d40676b197613ab8b75" },
                { "bs", "b174fc715431ac20fd221dc32403e392944dcb5118ca03918eb89ca960401881dcb5d074280427a6926a716ee86f3910f3dbd2fd5520db6fe4715069b95f93d8" },
                { "ca", "2dffdb645c71467bf0abfab0be8b70a5423142aeaf90e55eafa4c10aef5fdef6f8b4a427006a7c43ddd4b0c39bb8a5388b6758bc0a6561a1aa65e2cc12539a74" },
                { "cak", "250fef2c947d6e398ce596d020387ab003e2e92df2c5c68ab17d0e3e3505480774326c07aedd77b115b874e38aa4f47b3b8ab005408c6cea0dd418c6b95d6e2c" },
                { "cs", "a7538e87a2011fb0c8ab88cba37dabc5105be70a4fe2d204be7a12d246f5214636fa9e8801cf1da90e7368c9f4538fc67bfed12569e5259412bd96607cf25ebf" },
                { "cy", "76a5eae75f73dd252892975a98bd1eb6e5b6350425cc162350943e972ce7d8bd33b9ac2249d2c51e733e85b37d1b487af12a92eecdac4479051390d5022c906c" },
                { "da", "04757abb027c285789a1c58e8dbabc9b99900950abdf6bb4213481b9faf92ea22bd8f286703525e08affeaaf0ce32f1efa1407eb37dff0aab3eb63e73e52b6ff" },
                { "de", "d73c4ec2e95813e4eca685c11cace59227032b3930efcb20502f20b8ddcefeae821658dd2848ada8211c7673c3bb758456168b22b6ba5f6b1b0cf23f993719de" },
                { "dsb", "141f5d89383578671061bf9384a2eda16f074c0d96b76e65f733d858a0d6e91562c32ba86cf63e52115083de6665ac0d43d3b1d183ec78e938e548d4322f0af4" },
                { "el", "c9c7f964ecacaed398b362493cef04cbd01e23a79f2e3e010c4e4c234fd7facd5bce8d9d3700ba69b7bc1f784803416852cc6f3335366195393dc9d408b7182e" },
                { "en-CA", "cfbe1ada181283b97be069eb81e37329a939a1bbf74ebe1c977f18ce84f5fd7061db45a7b87534d462da61db526e79d74079bdc6fac60f9088879dd69189930d" },
                { "en-GB", "c0a2eb0de66481f0557a93c30a1e117acdc4e0b3ec15bbf0cbccdbc9da3f6212d5dd250ab94e9d61b0209b5fe666f4458f0b8836203fd50364ae5a4a2f54b6c7" },
                { "en-US", "2e574b441e2bcf3c60709af18cd28aef5a673c920d145ff80bf024fb25514c83d7d3beca2f5dda3189128dd583a7ce1443a832f0b4627e311a6e11aab396b14b" },
                { "eo", "805c842bcce3464f87fe9da6ab672c46ac4786f2184b5f37251ea0500d3ef9dc8e2008a8de47d046263108e706bdb6ae1b1782ab63c9b0be359d9440eb8541cb" },
                { "es-AR", "f8dc701ba1b33a31157440afa237b251ab480fc50231d38947e12c15a8316a9e794d46a4e8ca5582e459dca562b72bf364d341bb9a1e0b75d76b5f9496633e19" },
                { "es-CL", "ff9ea5ed6eb91e21e5344a7234399bd8824edab6e14d4e1f573efeb25ac10d47de7eae2bff2fd4c929eff87fad692d18a7feb3fa8e168b3c21fd33346b7e9872" },
                { "es-ES", "69d761c7bea65e8c72854dd0c55bdbcff8deab8da5d664e6c7f0da74841e6c83571bbe598c73bb3308875b863b36110b473b1ab0282ef20715828d6315e9209d" },
                { "es-MX", "51ccb12f17f671effa305971c2d9c087c9be7e6416eafbea33e33b36ae7e9b9aca155361aba603d82a458b3357c300d0ae35c8a5ab969dc2871107e3b32cf5cd" },
                { "et", "332c941d3d8773f85652acd823d97a1989b657a98631aac6039a00aad0fe82be23f0c551d7ae970cf4297e43563886e3645959d9b7ed5b3e3a70873f5ef010fe" },
                { "eu", "446298d5670316378367d79e3172db7f32a0e9c0a66c8183576e8d3f9084cd722d9c747ea5700e35f419afd4135f17e4ab8c665e248e1741f7e8f64788844cd3" },
                { "fa", "fd300c61133f2dc550a435e901db9cb36ca4a45135b70ec0436dd2a84d4fa111c95435ac6ece1a4d5a766b687d862b06febb438bc8f38b17b29ef302c914ed61" },
                { "ff", "d253692f17ace363aeae22a29ee942dce5f138d3434d3e5d354147df608fa6da48d4975e744e2a2dfacf85e4ea9ee99b184b8b2e04285b0322274e90c2e5bddb" },
                { "fi", "bdb3ae033c03bdbc454048f662764e9ededa6337cd53907e239839580060beb1eb2aa94a10560d5672cc9a032e7c76d042986af7890dfb4b7849541c5b33efd9" },
                { "fr", "b4018f62d5981ec4d5b63bf34920106bfccd73f726175f0f3dddc04988ee3f74241655d9ead8a096a9d844ba4af81ba958446815b6e5e47c04cf5df2cb165094" },
                { "fy-NL", "a6b90fd49f4f66cdca289cfae3323186686ed41647fccae6c41abc1176c413299937004d305f95105b8ccf59024d7e1dc9c702e46a0d3d5cbe2c9130c156d3a6" },
                { "ga-IE", "fedc2065ed2acb1b465d1908a91385d2a8edc6356d9a6b8cc0d7423b99c0a15e6163fad647714b814f3cd6a25571ddd1e406ef1839c4e2fd7329b1805ff7f11a" },
                { "gd", "eeb292bf660d3d8a2c4351a63ba48ca7e2fe6e4c061b00c16cace298282ed51ed16b16284df240be1c678bbca7c5ce61ccce80a315d56e1670b968353c77ad58" },
                { "gl", "a97ee0f4a6a5f5c178f13c510348508dc1ead4f5bc6fcb387aaacf7054f84c13c34508985e14f4bffd104175a9f20a9bc00fdb6ba1cf7ac7874fb0ba3a84dbe5" },
                { "gn", "15f270de7f3460ad41c99b3e9896534eb18750fc1936aee1bb8efefd267967d2140968ea6ff340408422feca3a1e05e455ddab35fc2ac60a88c31aca863051d4" },
                { "gu-IN", "4b19d926f43ffa55033fdab3884de03d3f794c288cb30f4a341d51e1cd3f318b8763fcc7c443559dd59aa39d2ae0a31d38491a16feb7efaddcbeec89c00124cd" },
                { "he", "6f87076a9ec8abbef34ea8fcd17435b67ded0d405c93f5b2f5e8436d27a6cc6b69de728957287e83978297d79b09c1b3f7f5c7a551c398e4b1dbcbde513bf9fb" },
                { "hi-IN", "6638c6a5868190867c8ad0bd30390ce359dbc5bc9a9bd5de3cd32adc63ac78d5f0281e97b954294a209cb718222177569413c177be986f2dbda1050c17b7ce12" },
                { "hr", "177fb5e541305816d098eddc01093f329877fc15cc588f1f6ad89999b7d32a8f3e899c6549539c415d9611eb94a9a67fdf8db3598db607f620f052edf0756201" },
                { "hsb", "5e28234c63700034f252e3e3dcc9f38d9fa28ed17d23d017166895f0a5e1a0ccea2675b5acde9b609d0be9461e7afd2ff85c2353f6f4da0609cc0d6aa00341e9" },
                { "hu", "fe6b1c9f9c7a5e17d341b9f6361c2fe6c6459375775ba66d0d041bc70ae748e133c68e3d28bd2b4b7c5f748915197ede8b7eb4add88fe2eb7f5963bf78337c70" },
                { "hy-AM", "82af541623fd7633014050cbfde925d018e1ad899f5de0b0555ca0db6fab948b9a840ee67c5919a4d0c6be951fffb5bfd0027ab6325bc41b485adfd5e7aaa725" },
                { "ia", "16e6058e4781724522ceb1033048fc7cdb69f25dbfadde5c0a97e87b26711ef5556785522d8120b161043d0e084598dc8d74d3099e842a7428ecd0180063865f" },
                { "id", "ab3c52cf7d8fbda1ae583da14c14a2f80ace78b8cb37d1e88223f84f8bcb321fd71865ba9221d3c4990e560f9ce1494a4f25733fc15703e2cbab0c5a17a18aef" },
                { "is", "b925f88812c90cac9d499170e9b65ba1b1bc60c9afe86cd07f7b0c143108f0c8bf0b05901b0c66e20d8acdf81d922927a78b7e197c83391a4bd4ce86a4b1fc98" },
                { "it", "949f16e2894bf31a695d9e278bc317926f8066d381abdf6010501b05d7cf5bc4c2149b86d2d91b2b84329979446dffbb4708643f21596eadf672c57fa8289c2c" },
                { "ja", "77ed836580d953e75df65830a0d4d8ebba85b5888dfb4db1a3784dd5dd709bfc853b8173b884d0a050a909d0e5e6d867f888a120db12355a7d4961e2aa721950" },
                { "ka", "2e0030eebb52247aca586e990b35cfa7a2d59bb17c17638112cd72c76c6ec800f410dce617dfcc20c3d4300cf17def7f40983d4beb6335fbd42a806b430d8524" },
                { "kab", "0984afc2c1335a95bfb378edea6f9e84a1f7a7eac3c1a7b99299227721459fcbe8b80710411619ab4c2fad35efa8d5eed98bd390305c909efc531106b1df27c2" },
                { "kk", "b2b3cc876a40211da48765f1fc09662880c342556cdaa283a8d98a68c1bd949e78652b1b29f2862a29c9b2b37b0f78b3c436b55ce28df0c18de38d5d2b865590" },
                { "km", "3aa429540bdb1eb3af21f6aa1ded8cf0db450d4dbe9da356f893011abd0c42843920fac9c8a5183227883df03fd53b80349a243c29325fbaabe3951057fe7fc8" },
                { "kn", "fc010634e222d8debca65649817926ae87c5341239ccfb699620b4bd300d7d788fb3c8a7a477a7659980b1e856f759fdfa0d54b7e384747849b60f15f4cbfe93" },
                { "ko", "de8e4cad20f9f6fcb777af89943d4e82141dc498136b0b4ebd15444aaa0d8289b73b088b07c8fca1b272a4bed3de92a8db97b5133e4a25275dfdce415f83cc15" },
                { "lij", "be509e9da90d622cd09935073f51b29da27827a86c1194eb4a99c590c238c37c7bcae70b6c99957ab46a6afece6bf6102a9a6f1247b4c6ffe5af70895c338b9a" },
                { "lt", "02090baa4cae10aa24505516df68e52b755829336bd0d5f724f72bb074627a8306fedff6979f4ee4f6dc33a67fe8aef84fb0776592c0d41fdb18196727347c4e" },
                { "lv", "37e813c946145109caaa2fd7235c964b324fb1add68348422fd6763c426a9cc7f5a4f17484bb2db484b167c22ee52c510e5dfb53b6e4825622bd937b42b386a7" },
                { "mk", "e93dfda4bdeab43405cd8d975f554c612203e2a9360c6c504b6f539ef62bba81f18390c1dabc1dbe418b8b0283ab857b0d7c1ecc8b1a0af2026329bac7b21f07" },
                { "mr", "6cbe3e2f006e48997d3d7048786dd33a1c367581e87d8b441f6adddf3090ebf43758f1a73cbfca3b2a2cae0b96b649f3e0ebc59737fba472358a1a511e4608f2" },
                { "ms", "d0c2f890eb7c6ffb3a93c3554064e9e89e7ffda66b1800c903894e1373d5dcabae38c44dc5545bdf3f27e7ea0c314a2bcc110cbf6fb6445afb7cca752f85bf13" },
                { "my", "04d03f21322f8c6f772f18b147ba2127e0efbb8bd7d33a235d8afc348b150cf125d20476a479f96b46977003809eb013ee3c0d321a5518cf9af8e61b6c1c961e" },
                { "nb-NO", "c3e61142180a462b5751d092139b13e26b94604007e3297f80c542a17648f5ab88923c021b8a1e57357daf3495366da3b0aa47dcc272589af3c2d1c6ded32eb6" },
                { "ne-NP", "176b7635c3fb55734229f735dd26e83b45aa9d3f234896abe15ecf305e64588a690a7ea4809e6c409a6a4ac143442247be5800ee4a49629eae4b7a9e4d3212a0" },
                { "nl", "2b801670b5ad64671d7cedef1e99e63c12f9c2d8bab0dbff11b48ed393a5a7afcd8f52a11da8fa6c626635c6cc1891ee5123593e9c1a5c2d6ce09fab9eeb2e17" },
                { "nn-NO", "8deb7526e3fa968ddf407377958052e0c37434120052e8b5f16f5a9d91093acafead30221426704b6e79fd2daa25e0d028ac588b1cdc012881d8f1c83f1c3342" },
                { "oc", "9445f668283f2311923049f9bc6ef0b67eb6a3712ea37a9f05d5d1bc95d903802a576a0305a38d465cf6f32d2e52cb81c5c3d57bcd1986c1fdb5884a685d806d" },
                { "pa-IN", "eae42d6c55f1f1b905e603372fb15198fda55ee897baa297ac8913da538118aa31de20d10ac0c2d4ec231d61c91fcf2cc6b38f6d19e220aa473f6ddb7667adf3" },
                { "pl", "cfda7fce9ba6be51eea728d0155364e6386dd5219e7d44295d998ff0fb965d5c9e44500b3ef544b384fe32af3c981080cf4033a7d62a65058525fc9598e870dc" },
                { "pt-BR", "1558320ae16c89dacf17dd80d4a45965516ea1b835b0426240ab379bb32a85a94bf1103bc393e089db530c4db0ed4d1519ea5d52174e68226e6d368cfd19527c" },
                { "pt-PT", "68fdf61f7f240df7087de2d565ab9cc5b4d25c17bfca324d8b0f7771131ee5cadc0a6c215bad32f1ce8fdb15ec799e045d55d3f3d857653c20a8472283eceb94" },
                { "rm", "37c98d83ee7384906b9273a44c44d5829b1c3301255df153451eca04882edff0f74ace7466d9ed41c10ef965d6ed24b48aea36a29b8013b0fdc6ffca4e647d1b" },
                { "ro", "7d527befccc6abfdc8af71870ee0b76380686181644ae690a426132099e0c248cbf053cde5b7820b952b5912e47eade31cbbd28df8de0ced2c3a412b2a502e8d" },
                { "ru", "1ccb13940112dda3e999b8b758e99c6d468cc0b06467f599eab91482add2935554fac31b4b8b8b6c76af8e8056a9cfabbfab07c91ffa307a27a43f8b97990d71" },
                { "si", "e83a0c7b9b5f6b610495c921741ac39bab0317f586048f9ab9c7e6b2cd336dcc2afc4f53a169869ecff5bdc28e9ff49084c39d9ac4aa99370a56afef3fc45909" },
                { "sk", "098bf4b226d8f4799433d1f988a76a36c98e5afbc5ab1ed79dd291d69cdd0604d7889efc4058f34ceee4e74461657b647dea38e7ba929285045ecc651e5f2bc4" },
                { "sl", "f4638ee6c0fc04ebf91d4c2558ce0783e4982636920fb5c4a7013da1d23b95528ca2c6da16914e846758f065183d8e8fdcc93f045204ce477b68edc47e94f2b5" },
                { "son", "cf97cf14fe19471812dfe836c4d853c09f4990211db98c24f5d7aa246741d16fe9c4ce0831c9867bbbc6fd757be02840bc606be5566db06f1cc82128490e8870" },
                { "sq", "e8ef85a4309f73cc227c19e32a485a008c246205f7e1de9b3ad381c89d99d64992a7b52d4b94a5280519bca4c82de076c44f9150f2941d345043c7daa8b42e34" },
                { "sr", "5c63697bf473eb2650d908de3108ad2b555535e421abb71750ef4e9354f60608810a95135825387326f9b74c4893d6fa88479142c7686bfd426ecc257b17d582" },
                { "sv-SE", "62d838cdfbf57ccc6ad68eb2988b56a07eec05a94c478314d76deaa02bf83239543b6fea0d497552725e1cbec76582ffdad4669f01af9c2c7f3ca66a6cb86a22" },
                { "ta", "ab6e8747b0104c509cc763ea1dc4d8bacaf14a9486665cd710666009af040a4ee7096ead728cf8d8429c1a8252a1e1c2b483cd0bd99b0829454ac4b901e59c92" },
                { "te", "6aa3789a14d3f595c7e9bee5316f024273083eb1b60991a995c8231236efc854215ce3d077759509939d67eb3405d3cf041d2b70b7bccaded748c788080978db" },
                { "th", "5fa186c26cc13e2e36434629cec99a44a26f519ab4e4576199fe172dca3c5a5215d155467c712b3c154054c710074177db55f9674d6cb312e965403d842d2d83" },
                { "tl", "7d7f9698b5e01a22cfdfbee7d85f5b0787c715792719da69763c6950a19b3ec599ca4649ca50ab8822ccd03c7e910696e2df351398c892cedd4877f8907c1851" },
                { "tr", "bdc12a6b7508d45f47b77c03737e3504f22e52d3da049f2146a17c17f751b7eb15d07315b901b87568ba5f9f121e3f8cf07e4bfe4433fdda66e81cbde0c5768d" },
                { "trs", "37dbaa8209587a28bed7d8e787c754deeb7d4f9dbe870661e6a525a6f9de406bbad2b72d6d4325f346bea3b29c062dc65083e37c25bb65999ff0a8a3aa77baf0" },
                { "uk", "33855a5a9f37dbaa550a2a3db61a7e920c008bc59288b4cc78e1d2eaf03a60ea16ab6f6b2dae14d242b08c30b17847e821d31f6e577ae446209fef0b0bda4e95" },
                { "ur", "5600b61a9e5e8a67d4679255203af16bce220689fd83ff636ea8a9f377a6cfc091846dfb2169730ca780c2c26258e1868ec883ef12ee50a66b65163e56049516" },
                { "uz", "24f98f867efa736f2ea5ab1920bac61b6a1fff74db8659400def430cb671f8860ac15baa514d60c7b8b2c56679b8777673be60fb4e37f01d52497c52f6937236" },
                { "vi", "d1b786dadd526094686b72c4c3c47487121f2ca521ca58e320259bf2951e0cbae57ac35f44fb912986a11451b33a2257653e81625895b6982b8502371eaa2952" },
                { "xh", "6c1a8b990bc6fe301f9b11fb9155a57a1dcf7a9e061c9430cf75c3419da3adbc84c420d5dc9e9d6097089f6323b8acea6cad16d4637f983f73bf01860f03483b" },
                { "zh-CN", "0f91f29c39b87ae0d67f8e76ef2f376d21f413425ee1bf708381f7dab4c41ed025bcc36c6c31611d4d2be79dca986be3aac17b54a990f5946497820e104028a2" },
                { "zh-TW", "e8e323239f6f3c717a739af5b2f8e52e4c2fefd01ed8c78e4d6177d5357a7567ef5d9376d72118d8488d4290b80e4e9054dd863690f73ddc94b9ce4e89d8d4bb" }
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
            const string knownVersion = "78.11.0";
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox [0-9]{2}\\.[0-9]+(\\.[0-9]+)? ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox [0-9]{2}\\.[0-9]+(\\.[0-9]+)? ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-esr", "firefox-esr-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox ESR.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-esr-latest&os=win&lang=" + languageCode;
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
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                return matchVersion.Value;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox ESR version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
             * Common lines look like
             * "a59849ff...6761  win32/en-GB/Firefox Setup 45.7.0esr.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "esr/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox ESR: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksum is the first 128 characters of the match.
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            // Firefox ESR can be updated, even while it is running, so there
            // is no need to list firefox.exe here.
            return new List<string>();
        }


        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox ESR (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
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
        /// language code for the Firefox ESR version
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
