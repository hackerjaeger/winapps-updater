﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022  Dirk Stolle

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
    /// Firefox, release channel
    /// </summary>
    public class Firefox : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for Firefox class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Firefox).FullName);


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
        /// <param name="langCode">the language code for the Firefox software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Firefox(string langCode, bool autoGetNewer)
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
            if (!d32.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            if (!d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/firefox/releases/105.0/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "9ce9619222244eb2adcb8b036ac2d665ca674e92dab5660358f2a1c54531c5d5e46df214fdaf7a71fbb9be2b10624a78695a3d7776c92287f4794979cf4ca315" },
                { "af", "91e8200ba878baa4dcd26746d0fa7a583625fd5e35fbe0a46e8fbc7acf2fd42f1f401ac7d2dea3caba9374313f73a7ee3aa1a4f6a5ffe134ac4c8ada254692f3" },
                { "an", "9fd1a966048d4ad8bf28b60c63e7ab5c08e4f51c2c123067a5ed1e86424b08f11b727777da9465a701dc596216c95950215b86c8dfbeaf6f53fbac83307a2671" },
                { "ar", "34f3e1ab2c90b2eb30262f0c05ddcfd77d0b8fed22af446402af2d57fcbf92a79343c1aa3fc16e493f53e7c7fb3652edfda1ea322c70d211837918daf05c972b" },
                { "ast", "900d901fe5bde0cf29ba0b3fe3552d4e194d45a6c0d02babccc269f303eda6c3efc0a9b79fd6b5295f4513f02a35f78217baa37d60967c017325a9d7713f858a" },
                { "az", "3dd7fc43217e61ab1a0a5df57ed0e0d7ac98892195414c804b3bc4233b3920de5a43847a27013f31c3d4ea1fc025e1f1602bd22fc508b9884ca500ffb126016e" },
                { "be", "80aa6e7da7ca363883a78be9efee1cc84f22fb24653ff053910a06dc4f949c61bda7bbe189e08f7e78a5d0a35a8a91d83038b5eebedcd5f767caa25580750a93" },
                { "bg", "cb5cc921725c8d7e5a9b568db61570e3b8123f9dd0391104355a5bad5be416d7a4c8f08e18a9f66341629ce20d137add684b02638cd22ef368cbcfd7aa4e7977" },
                { "bn", "829969fa70711c24f5e6253417f85938801c42a4f2b534400b0a7a6e1f9bf2da2f47680830806040568c53451574cdd04292ab6166a78a85b1d7ff962fcf7397" },
                { "br", "eaf0350c00a46ebc68ec5f41f200562728eea1894b60d11fbd0f15a31848e7cc2f594948b559190f116b56fe7724fdd82a037dc758903cb9045ef7ebd7e4af4e" },
                { "bs", "63266a9b09bc8d37400720cc6ee06f4337acc6ea4c858d1be0d6dead1aa77d856b77c41498d6ff225b30d7662b818d3fb185c646842c5f941e5ac923293dcc54" },
                { "ca", "3a998434656c002929f908e227eda1d14e1b5662469a349e1a49c94cc334118be07ec8699f76d8f2b40d4b2ee822cd20f1b7ae644d4f684954635a8aa2bb2801" },
                { "cak", "dba304b4fa83797ea21b991048fa44fe58782010eb7f9156789d78d2ee680fe523275c33799909f11b47c69296be87e7efd823aa5c69d4cb2c7724b86cf7d21d" },
                { "cs", "83050856b336893655023a4cc82b12bcbae0c54a9beca595e78c72325c770c40f2b58f6dcfced74e5582ed3e072924d772bdb70d40ef83e00a04a32db40d588d" },
                { "cy", "d12642f0d74d16e3040910cdfe9054815355371bf60db484d70e90ebfcc484ad036bbd40f19950a33f2b9c8385fa1344bd37586296e9ff8338c69b18d8058a3c" },
                { "da", "165d8ce0a577db1307edc1c358bc615be96fe25d314c7cf282c260444c971b3d445bff1c4c8dc85b50abf5e974fd6ac24769b78c3750fe7934bb1d26f6425c76" },
                { "de", "9f01379566535c8b7b340ee40d4407dd063d9a26576e8b8f9fed7b4ddb573202a3d9c6166e69d3b12ca1b0411db8ac0076596ead1422748abf326ba30bcbf562" },
                { "dsb", "9bbe68776d282735e2bdef76e14ac4aa6922aa2eaf0704030a8172698284d3ef7c61439afdbe1c1ea01c6472ce3a43c1b293e9e11343043cc40f616c331fc434" },
                { "el", "d963240c79579a1c5ec80200fadc05402ca2e9437900b95a3ee5aec155146ae8428f689a4af1c1f243fc399cac4a9947dd03e6603ae0c91cd60f1bd052054333" },
                { "en-CA", "ff1152b634beda30dc7b404a1980a231259063647a20164d5225ac249c6bf331438db4a419447663857d579297c19ca1af4d23c93684b190cbe326bca0b63b4d" },
                { "en-GB", "42faf0b1ad6df0f2fdad5d7577bc903a01de4417a9d48e3323cb6d54db3d4599d60c8e64ff5f9c1c343ddb148489148292ccddc113157019cf5e78bac678071b" },
                { "en-US", "435e24bbcb4f6b11b60306dd2a67c0306b376ba47fbc55c209227e95303a39889de506e741a1ca71f8dba0e730845d09176b36c250b3b8003d9b5c1589645fba" },
                { "eo", "71aa843e5906224c8397d1870f142305e874da4875ee3af99d41b235375fe8782988e33126be23c1ed660028cce16db9ad4f82887b39c86ee60bc569a10b4b7f" },
                { "es-AR", "cea12b74e294a26d435ba1f513a0e0754126663db1ebc8d969d8d4c4a3e0ad08d234b5638876535f5834b4dc8ca54e9bbcbf9c7d13c9e7cd7c4ffa7c7dda358f" },
                { "es-CL", "18c0ab6a27a72572caa7383f5204d83e1eb6e7bb3d246be97e52771a0e79ca9c0c6614e0e1de44af1e8f09c1d1596028e80f1cfa1dcfc6b887ecc41558245fa6" },
                { "es-ES", "091ffe3a9b538adecc16c50b8fc930ba8b0c4b41d6abc74f1a5176d27394949ce25511f16297cb55fe4a7cfa5233461ec029062fd7fb2ff7241d841a81138eda" },
                { "es-MX", "fd6851cf3120d412eb571ea00c4d29e693eb7ccc5b0e1191ce3a9566fcd261a7c9818a6cfa146528c1545d4e86ddf0860503bb13ba39caed236029deeba59865" },
                { "et", "2000cc2179bf321c4bffb2b84178db413f2d265636868b9254540b363b7fc46d5d56072c191c13bad84b5ecb63a1f46a519da069cdda427b1d31558236a99ce5" },
                { "eu", "5eb2cc24ed0349d2103012395e4a933c47fc73f239c2d9d7f94ba3ce5b2ff7a6b84f14e0c88dbeaf11aac6732d31597c7fd85104eb0798bb14cde8d15189adf9" },
                { "fa", "debefae7ef8955d2e23d154bb2ac15fd5799f53cd5c83914778fe1a5293209f0b9d4608a2d4dfcce1d544d1508da57f7cd6a0e782312bc4733248bc65e6fb575" },
                { "ff", "bd0e440b380efce167b0cc7c38f442da7205be0119918b8f22f78e7875012540b15a9116874fc954590f3791484c00aff27c70be2ae5179b44cd5db0f945ac2a" },
                { "fi", "fa811a769c1ad5bb3c920d592c10dc6eb1bbc0b7307490cb3a22bfe1cd1a3da29d5ce5c638308abb71fbf8e645e41a1bc17235d2fe465b7ff62109ef8c71fba8" },
                { "fr", "20b45eb1950375026e8933bd59c9344c6306377dfa4083c15564b7cef3834fd1ceace997ef32817c29cdd463f65f2cf9005ff7b0c37e89f71f44259f293f5165" },
                { "fy-NL", "6e824c629a6db44ac5ab2c69d597262a0c7a33384164a564a490e7991a462c0a7a6747735c709e5a088ddd70f23401bb710b74512999728b796f883da6c3cfa2" },
                { "ga-IE", "0388c85a7d14b3fdf7b32f9edd19cac5a4fc65dab9a9e63118ed47dbaa7a29468023ab3eb757ca7f58180dea2aed4a34b907a48b3817405d568f2ecd641c9c56" },
                { "gd", "d429609f1bc9cee954a905185bb6ec28d8000a91ff6e8c68a3151bb03241cb822526c80f502cfddb86747cedfc22616ea3cd4d07958709054045a085aaf553a7" },
                { "gl", "3407bc566ae86c0d58fc7add21717b7f564624df1f54758dc5f6adf8e32e571299bdf0c157b03d7802b947e75b28cadd45a840ef35a7e1f76d472503b8c2df31" },
                { "gn", "988371819cb132c2a59d7f1367e59b70a8ecad7f45a94bd8ae0333a09c50cf93e363476d2dd59b2aa7e4b35af91a111d2424a22af2d153590c3f743b065be17e" },
                { "gu-IN", "11e39d22bdc464c17f0e740919e6c8b6a8bf66d8417aab074e6ea69c8581ea80d65a794cf2fdafe98160d1446b5df08048833c13084fa4e128e2ff39dae1b06d" },
                { "he", "92cb743532866d41da9ef2419565995b53fb6c7b7d0463b468ff85875d8a59dd5832d2685bf9ae4f7325d860e57a77ecb19ab1471d39e1e554c8c968132633c5" },
                { "hi-IN", "2d17c2a8171404e088834fad423cf777d7f18d510e0e5985434c2cd5e675b1ec8a62c594429fadbe3eac8fabc7a06baa9dc46ae15554c67db5a3499a5d79c292" },
                { "hr", "416c1dcd5c7be77f2e0afe0c7370a29afc7c5b18003139e11b1d5e705c9a6e0388a3bc694bcd073a4d8cfafbc8defa6d1e1b740f79dd160b99250700c796f875" },
                { "hsb", "a5f85b1b3eece2a6ce0f52cb487f1dd248d43aee94ae310cd2842a027286e225c9df6b1e207920efea16a256fbcde5c8a8cf50dee558c1209b18a976cc180661" },
                { "hu", "a460561410a57c79dde37030f7d9df808f64bd86fdc6bd89cfbadc093d55fc7aaa55b8bd7890d7bcc9f86b11f4d47887bdce5ebc172e3aa69adbf893fc00b42d" },
                { "hy-AM", "aef9d000505628ee16c725ea079836004c7d050acd9bc03f399d4243c2a77ec01c1c2d9e356d1435cfffad38a9b9ce623c26260bf4cd09d64e7f672334b583cf" },
                { "ia", "ffbb330e64fa33d1c2da1f11da306bdb04ae4eeadac33c9eca3da06746b8a8dfc9bfbd660defb1a78344d9b6265b9b92106abb73da1ab3cb726ca4bfe72be6ee" },
                { "id", "9a5ec6993a8bffc2e9b76199d5d69dedc4a9c2aebd823d01ba3a6dfeec92b8c71fe33c9252dfe99d3160d633ec909dea9356def02ca9b411e0b1c18b8b094ef3" },
                { "is", "7fff5e7edfb92b44b1609df43c7e284866ef083749b01e27a72b83d8460ab620d3616f094f75b2e2747fafb97f97cf60f50295d856ce4524bce289c1a5161ce1" },
                { "it", "27f95aa8c6b18186d5b26cc2cfd0e204a8aff7528cf60d9bffca02bdc6e26f6ff3c32ea6af48291ffeb785cdac64e88895ffe50ca52a2da90ace7558915c416c" },
                { "ja", "2c70b55125cbb87244cb4f62058ae20d5193e80b16ea59bb0d569de3da7c42fceb2c4bb80516c4a20f2078519eaae1dbe03ce328bf047accac9725b67e84dc7f" },
                { "ka", "23a54bb29aab3d0e7aed86dd989e6b46cacb647695e93380ba8172ff13b69a66298cb3366d69bf45c6602de8483b813cb12fb56a43c2e6e5dc33959042785493" },
                { "kab", "b344295bc10edb9632f414215b4a2d278a1580e534b07f48f60588271f2d27faba43b378d8d855e1026a54ac32f8b94ffdfa07b47b6e87a797a2582a47a004f2" },
                { "kk", "029fc7389c8fe4be31065b2ad1c4d24af7c71a08f724d971744c306c8d1583a28c523c643fe9b1bdc01c925b5f834292cec2ad8a658776974a5d09d318b07184" },
                { "km", "2557fead22f03b0ea0c3ad2ac4756ba1d9071137d30fa3cb02d760b6214eb947d57ffa5600ece37320b7e9d3a972a37c7cc1375ee1c7b0769aad6afc7448bda4" },
                { "kn", "b8bb650e076d4d5fc72b92bfb142ab2dc9fb7c6797a16a6fbbb07f63eeedb4f9971aa6a42dbc3d4c894dd7fbecd9568a8f880c921f34f893c6a49fe772fa675f" },
                { "ko", "dc926b6568446ad9e552ef4a35bcfaf5c12c7403e363c3efe82065ce152db4b70cfc4a5811e6155ca5d10e06370cf10826750c79abdc504e098c4c18bda3fda0" },
                { "lij", "a801bc1a0d7989cdf213909a87daaa1a1ce84b285820a669a08280c55bd5e3b2eeadaf4742992c9c12bafc1f493fdefbfc7ce9e1e96a72b4b2e0d0cd81c0cd81" },
                { "lt", "6d254252d097010e6fb2752313359341b89ef505eecc876a74f8e03e12778cbbf1570d6b147cef5b9217523933de2df1ed9eb04060e13645d218f5e6b483e226" },
                { "lv", "3004304a4bd8102ee7023f5099b999d427d12f388e1dad90d2bfbe0ae93c8399f4d024a1b607b4066067bdfc3e037aa67dc258d1ca56e55777befebe52dfb701" },
                { "mk", "7649d71a4148a3d6f5bb54f043e46ee800d9ab61cfb31b4d4673d6989788fb3f247de7d8a63a1c22ac2a98cdfd3e005b9700c131cb5ede0c5bf3154d046d87b2" },
                { "mr", "d1dd13066481e668d95db056a4f0cafe991ad136e9407ce6805a67379c8bfdaff87b331a14566b725aae0ed86a0a12b9c6b1cc6ad9aed01a8679894c5f855583" },
                { "ms", "788a1d8afa2a2a8610aec2f3cc8487b9ec4b462ae51fcbf5a2926ca5ce20e38343c02b404899dd7dadc675ab5cbb8a0ffeb8f610aee157d3ef26ba812842c39e" },
                { "my", "4946e05d3ca26499123f07444f34a0120e6a9ab77c04945ecf74e151cbd0b1f87705f3abe18bda0c542c050e234d7f386b1c0559821115f5a5cb8363301ad784" },
                { "nb-NO", "8cc1840cc0e88d6b0d9ca5d25e5eec206ec3ce275160ef1c692dd9b7c05f864822d4d7c357df36281877b4feea082fad16302b65b94b433a9ba2f928997b74ef" },
                { "ne-NP", "5c154a1655e22877564b5ff688f5be0bebea6e4041b9df0beefb2868e84249126e7efcb4cff71220c6e8b8368a23dc1b1a0c250bc90d01e11b4f4bd5bb65352d" },
                { "nl", "dc5b16c318fb3bb2e6ccd0a439f1eb8f806ab1912cd8951c7981f9293d3b055f32b47991d0c43c92301a6519fca223eaf0a661e40dec85af025d39693720a9f7" },
                { "nn-NO", "95ba981c8512d6861bac4c99526a9f948696738c867ade03966e582ebe4489c55ef9680990ae7dc6f619df927d162d273f40a73a4e83650ce6dc0a9dec943325" },
                { "oc", "f783bba9e0b9a44b678b67c55aa4a7890de9f5276e7653312ab69c066cb7c1e00d1ac5b1d0b84acf8a314e57186131ffdd47f70220e8eb37340c7ae0bfb48b43" },
                { "pa-IN", "239d103e402ec4a1b6d7038c9d3d40fb42fc6a0298185479d42393a8ada2ae44cc9121a8b7b53311f61e59526db7eac341191e002027b642bd367e2645b87902" },
                { "pl", "849358f6fc19abc70013a5f996237e46b3f1f4d54c0e22a0af2dc435cad1baebb1f5062a4b3dd216708a0336d06dbea2b7a7e16995ae98bd47181caf6b7e7b39" },
                { "pt-BR", "05305d8b61c02ac6ef852b552826efbdfd24f45f1998048ef4fbc6b5d1f39aed8e37576665ab8f5c683411b3286aebe764edc92c73055b5c9cdc77b25855912a" },
                { "pt-PT", "5a422c60d68b756032dbc510847b0c1b7def5615677176f18bb714a902e4812792b17b0a20ff139fb91437e5897cf27727a112c71cc3807da2e35283e4a29114" },
                { "rm", "5e48e81b26b1040fbe20e4f74649d061d9cfcb1f74d49adbb77e0f0baed7ffa99b763a741213cbd767b3310a99528fbde2bd6a415146139c1a003f8128950012" },
                { "ro", "8355bf8329f8c96914860974b28038fe2a0c7860269b2f35ebd4ec348429c00e6e7728c85032a26ebf8f8ea52eacbfb149701aa8511b59eaa49fa4c8007f2f1c" },
                { "ru", "46de7b2d56ad178989f6980ca3aa2712fb6daa23f461c55fc5a21a99813559189680fc50eee808190fde86ce871f6fed0c9e5ba9a3709705501e307dd3be9344" },
                { "sco", "0f82319527deda93cfc00e870ad0fbf3f81a98daa17307b24a3ad8a7e76b25a01070a38baf52d573ddc5b62248f2eab72c59d3530a5a509cddc4553eae64b7aa" },
                { "si", "c949d1ce31fdf87450265e2798d3f31f4f2be5df100248676493fb097389e666f3f6c16dc870e1c8d6cac2cf35c7a6a526b2d439bf1b2286468893bad8ef327c" },
                { "sk", "6a92b87ff3a103067f1e508a8c4c6f4b209db02b838d7fad5e19a1030aeb9ec02211e70693161ab42d7757fb392cfd2b946e9bf4eb8e32791208cd8f20c08f03" },
                { "sl", "18f7880525b26edfe0f862fd39021969f9f70ae5125d90dd538b0aa5dab20ea4e23eb978d27153b29d6bb5cba71dae4e8ca399f877b7b3102e1a3443fbac718e" },
                { "son", "4033f7d2cab7765cd2c8601287a5fe618d158cf9c08dbfa4fb41a834bea38435997f91c9812cc4aa541fcdbfa39b34dcda1a2e5ca3253ffeb0377b5e512d3ca5" },
                { "sq", "a3494fa5019859fc6401532e5d077afa1c4c4cdd4313c16e3ca74f96e472e8fc4e7cc6f319055db3ae9b96d707154cc6ee0c87278d9ffcde956b23577833bdc6" },
                { "sr", "435f3e0f7aec6d33ac5206fcecd0b537738f109ad93dc7bbffcef89b6c771908eed9428707c39bfe73712ffd498df405a87f182f924a5f68adb2afe99d4187a6" },
                { "sv-SE", "6d1fc77e0350f79b5c54a34cc47361b9f34530c6cdc87e5e6975022c2350e79acd4dfc761388353de2d606ec30cc77941e5840b96a6931dc1e29d0f93e713c7c" },
                { "szl", "facde7fbfe298c55b4faaf9a965742a8231d288d52829f483ad43baa17688d7b77de3d00713ea07455e5de5c5bc0f71478ced3f4dc552cce3978957cf1296219" },
                { "ta", "a245817ea6f8191a99ed65da06b55b7eb2a42fece12c0ff194cf9350f92b704f3ba2d8a673629917581511409d0a6f678df230f2d3d2351e3847ab12ef0df8f9" },
                { "te", "ff6adddb159c4e4ae13d3ca064960a0afbd18e94a426a8dc283f4409125e40c2249eca82e3a567b21967dbe6cecdf1cac5d23163897a9f96e72443d55ec9797e" },
                { "th", "d8f4d70d6b33108f167d1ef637158242d3ff1cd83d381b72e856278f57b4f204f36a27a8896303c2f13278607e6408fde9718ca1dfa4e0790eafc48a56982674" },
                { "tl", "265798609e810f00a326e17602ba2eff455e9e1ea5307f49016834ad88e043e6ab73585cd2da3dab6683aae5adb2db96b528bca314b4f54e4b3fae24fa460434" },
                { "tr", "18c3bc6027d10462a6019a1f0e1c404768dcf3d0ffdcc9fb73f816d696186db9bc388c07e074f9b0b3dcbf01922bc2a0cab629b5fe7d84ab6fb849d934b151c8" },
                { "trs", "56290c94f17f36a22ae2e543ade77b152f7758463adaa74161f4626ae432f856b256c911b99f568eacc8f2992ef308a5a46c931eb79ee1e64c112fdba4740d2f" },
                { "uk", "81d39965274da3065407737a6db64ffb3e2d44e2492f241e4332276eca5ad3d707f474631a0775527f27271b9b515f3722f44ce65cc2dc55dfd7546ac50d1219" },
                { "ur", "11baec3d60a538a217913c851e92d724286af087da89decc655eac5bc3926763e4e2d64e5c4e209648acba9c3630a44d6c00f43e1a6f1a060f8e0e79bf8dc480" },
                { "uz", "19332259ba8971e47a65fbc12a6adedad915616bfa1c7ed9a4efa437c420c772b687084eb567cf224303e2205e3a2e10909c1e418445143a97c656a199db1c8e" },
                { "vi", "d502c3458602927838fc597bbac33291cc8bd9fbaac28d48cb143e1eb60db8258ac739e97738b3deb2233345c02f67eba13b23e1aea4bc5354caf085661b143a" },
                { "xh", "e770da8ba802b32066c657cec8cc4342115f2879fa94d2b0e707f68f87edf9658508ba61b2fb06e7b0d98ce0de324a4ed2726efa692179231bf90064b79a77b6" },
                { "zh-CN", "81eb4b67e6700fc4463ca0e5a218df5f4663fbbf475d8de2d27f26cab63e1ee19d43e11f9a8ce82f07b3e912357f29a8a157a5c171c200242dccde9dd7ee23c5" },
                { "zh-TW", "6073b7902ffd58b881e9978a27dc9cc25271d7b0d933757099c343867ace223626a7280b1aa423bbadabb3bab29a39e61d32f17ef03cc9a4179a75bf14da340c" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/105.0/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "fc1fec4a023c9cc6811706b8d92a5532d3ba31e1a0bf5f9b6613c848e7ba537ba3273099a04db637a876de456250f08635fe4348c254085e3757ab9887127284" },
                { "af", "22e69c42d89151249d9bdfd14d5923ea5a7a4fec3bbbe676bdb113211eef26215456f5a54de293a6f38094211d3ef502f5a57e745a67bc873397169b78389f68" },
                { "an", "31d483fba754990fe5fa29b690c9b5f0389154ed9507da00985d01656379b7f502391a7fde7c1edb31ada8c2f8cc7e47206af294a4780d710771dc7f9ba08ef5" },
                { "ar", "911b0a9e3993d964dcec5890ba785fab9a16f30c5558e01d4df45ec297de676d0ac486d72139bd776f41da02b0cf5578245abbe7efbaca4aef62a2cbcce36400" },
                { "ast", "461d79f2d5b0034727b1b1969bcc64b39aa5346a59b46a0f9df675fb2674b2ce44efd682f8782889350c410562fb0e5ebedd11fda5dd999cdb277129d3f55ce3" },
                { "az", "6b304b5830400f9afcc75360809b0c55f45fe41243b19682719b111628ebc8e2bbeaa8ebb819de23e79ed75a2bb6ba449695da410b24673cf0ffb48465312ed4" },
                { "be", "c0e8992399632b36ff29da059a09c9b5569a28a0b5763088b2923b5e74f9f2f628ec6bc7bee37d2555bfea84d34730b1a28dab7b35e712f79bea2496094a1c6b" },
                { "bg", "30aa8898f475b5d11757c1442b3033e7bfe15f8f04a61cf51b6dabd42658f0fc255aee16254a707ed354c496e92c14c8e97f65b7e9a2b2f7ea332d018f86dcfb" },
                { "bn", "595adb571d4efc543bab681e351e7dde8534196c416288d1df6a13e4356d48824d079b93b8703dbad35d5e97526be7b6cc5b2fc1b27ef4dee586e2ad55a4dd70" },
                { "br", "9bcb597ad698d235a81638e05c440a462b4fd5425d8e2d7a7a491a5713e618748e200c32ac8331d233dcf959cc8c82d608e4cea9a173b58913f91d9321e641c6" },
                { "bs", "d200e7e00b796ad00eb61a4fb733c4fa3fd0f77338c8b3c9e853d3975592ebb238d83ffb3ce7d5ab605b84cf03c6308c9269989cd66c9f7ce232e74f4b45b8cb" },
                { "ca", "eb85d362166c67d79d098cb2230dfa7ead49389dc313f7751e8be6a78a1c56cec78854e0e0f71aa148eb7b87fd38777bfb7dca4ab31a34b2f03efab6e1a252f0" },
                { "cak", "c89020404e50aafde726c42ae290cd5962b311fda0b1da62f52b5692c8d7a0954f6d804127c3b5f0b89da39ef8223db5f53723033796e413472ce695c78f951a" },
                { "cs", "37072faeb3a5c91240a3ae7444340bc9cc9634aeea94d05548453cedb492b4cb861f538dc5e349d073f3633c4465cdc158bb02a2d4f1c866994ed1f9638bc04b" },
                { "cy", "ac672d72d630d369037ea934d7f8414e650441fb7114080100b1a2a6b873cddbb7d535e7ad5b733449525978611a6d46798defe87ccb998d66e2326bc0cefede" },
                { "da", "c5c78c6176a2f53ede7ffa7f53ec5917d17b7f9952cdd4a8f5f284fcca6f94c50ef1a6679814ec6a5f1d39b6f6f5b7b0af54916431cf34405e9d5ed7e5ee2a3b" },
                { "de", "713d2af6c25e804d591144ab98f65418993ee6aa7f4b602e40ecbdd604603ab50569aea2cd16096a5237403429901ff780607d055f1ef6ec373ede786942a6fa" },
                { "dsb", "80f9a9cf8ec0afc57115d5de31a35df4308cc550971e107c1adb43df20b6413d26c4f8043c008c8df94b6a40815b31792e42e67c2435ca480fcd4349ce5482c2" },
                { "el", "887255872dbcc7ccaf849a8fd4d59c3a9db618135a65c08e15ca9337a359bf7f5ebccccdc1edb6d7c5efb34565b69bf88d8a3ba6cad3043a92ffcc1d2b4fb70c" },
                { "en-CA", "484bc66cd1f04c704da29236a3faaa2cd0a92260a742b5fb1f36c54d32fe6ac0dd02bc98b3116d0ab89754f876922947450644129ad4b2d1ccc5361adeb320b1" },
                { "en-GB", "85d985c58a33d49d1261ee7e2912313f0062a8eb74f2d8a8d51583a830637d330e4c56a438515205d1079cd7ca6013bc48673e940f0787cf5af820ec9b392b18" },
                { "en-US", "a6ba5911114b2b593fa325ffc07a6cf2ac980e67ce9916ccf0d4becb6dc8213cee3c53393f1e34a6ec93da91ff395d26785d2b37b010305598205450bd24d47a" },
                { "eo", "76f83819c18b00130b435a96d01eaaed49c8a2cb861c78fed024ee03c4cbd4f51c48b43597822c4327f554e8c2d25a46d93e2d790fdf35c74ec892df57cf1540" },
                { "es-AR", "6fa45d6b06739a178021ba48f2b510252c52bb40dac7b0da8babc1fb5684d7f8f2ac2d850d1c5566aaf58f6e14da4afe330827b035a13baa59db382a4492238f" },
                { "es-CL", "29aa9b0ab26b7f0aeb85dcc000f5373773ac312e4e31da18e6695f9183a8a2f1801f5018c70013e8e3dba2cfa750ad1f81e4854bae11e6490fe002b296a1f01d" },
                { "es-ES", "fc20f81806c4eff23a1540f1e06447437e1d0b70f9bbab8b998f3a733791e62af1022412b48a4e76f4fe82efa288e627e3a77bcc1ad8c9ec0f698b020f68f1b6" },
                { "es-MX", "0090f070ddcaf31b872da62794f72953c6e3b5ab5a7db767a758eb5fcc34c931d56eec32d97440e58f85f1ce6c605b5468f1d09294cd89cdebd77ff8491fbaeb" },
                { "et", "d2e5c880fd7885c70859d1d167fb0072814e0caf027b4355e8aece3ae3777a5111c8bdc44fee65a6991fab6224452240f9345b2a7ea2c4c6ad4625b5ee5f441b" },
                { "eu", "9708d20cb12124b4b2fddd7200affa9d33011bd93d6efa25881ded8cfd42e88cc7fe8af4ce94ee17948e3aca05cbdaf12289d5d94e60617352f97fc2316623eb" },
                { "fa", "a1bff6c09f3cce1152ae8896401cfbfe278b085aabd4b3af02021a7a1d5773e7c9779ef8f7969e8d01fc920bd85ed93ebb388db9be3dcd163ae8b2b317925a78" },
                { "ff", "735319df6b0f0279f340c11125202d99f064449f2819c864fb751bd7936a492be30e2c44be640a955f84eee9f156b73b8ac9c8eb3424775b2afad250798b0571" },
                { "fi", "7b81ba790450c5b1c9c9546d199789f2e272f10c61c0d79d3d1947de944e021338e99b361374b96e313f89dda91f9efe8b380b168dd60ea2ca91e8d729a25ab8" },
                { "fr", "20a61231046186d0b399c6c0fc5d06b298235a82f29f0860026473b14a301f721392b8fb54de862d155a66089d111f56e5ef0e366e2608f1e41b5736b61c3543" },
                { "fy-NL", "3e7ec38025f616052a98bbccfd11fa5dc08af88065d7dbd503c4542d0f02dc21fb49a2caf3f319cd32724272435828dd5b71e07357aef1d72388b9c5ed1fb869" },
                { "ga-IE", "0a84a7ef43dff406960f9998e5a1daba5e0727fac71b6813cfb0546bf2a11c82e796d8ce2f5a507da49c9c783e8870acfa59a4b6c72bafcafe35d6338c79942d" },
                { "gd", "357dbb86dbb068f34f87ba567cff05eddb94eb1e4cf80b2c834ca8510a1104e42036e9ee5dc13e64f66f6d59ae4186f028f7c21e6bdb1409f10e45f6e801d8d5" },
                { "gl", "4c078fb9438006299da06be64590688078934462778dd275d80c45ef9a76f108714e04e47bb8d03009986598f3cb40c8a655d6d05414010e3b427fd117991e2d" },
                { "gn", "19754f9724423a605ec525766b3248e774e97a6822f68d96962ac85d12f920e448ee596d97ccd53af0d5924c826d27212313337c19d56bf632b441db79757835" },
                { "gu-IN", "edb417323c24fa5145e223e6bf609eb12aaebb729b02e2ba351df95306e25b13582b4ee440eabdd7168fb341ebf4b78ff4dfe56cf55af9038ce2863ff2f15043" },
                { "he", "702dd4057b9339ef6c9675b28fbff97a7e01c401c62392109ee00c9e0441ea50e88b28ce159342f6f2cefd5ea0563e845e52adc6cc20dae0fe2981098cb5390b" },
                { "hi-IN", "d606c8322ae0e87b84d5e281037316bdbb144bdfa690859abea391f9b7bfa83a6046066f044f637d1e3defce2980b49cdd40a0ddbdd75ccd0c197cb59b298beb" },
                { "hr", "c8a2acfef8413b5f9c2939836b2decc7cc963faf093c488d341ed8b914f7cdac6770000e553c2d2bff8a3e7b3b1b9f310387c57e2244179b8e291de9f8105138" },
                { "hsb", "383c57e36d2c7d42b03b6a61b999752e59e360e50b501a6370047a42b815e955e74f985c0680f701ca4cf6b2c15c581dec275dcca79f26e51597d1c9ac4d82da" },
                { "hu", "15bc4afa26dc1d029e4b47b4b9ccb51e6cb3a9dabe3c90386894b2d2f49c0497e75447710fa671876d2733beb8bd591aca6560568fb67eae119e63df51c1b3af" },
                { "hy-AM", "dff4394d80af1a1d98e47a3f1be281b49a099d01ecff6eb0f0d12280c2b9258d10480b28314571e6c8bf5739a89086e8ba191ef5acf80b560c0303aaea11e3c7" },
                { "ia", "ba06baa1e0e788efd1f2620e3f0f2643bed4c05001fa5dbd2225c3a9ccfba3d6954ec5c99f10526ec78bfa0c8aa1a8111a60ef5d68ecbdfc524896c0041f713d" },
                { "id", "c0e95d0b2a371f7ed0ce4d770a2c8e80cd92ff8d9908adda00d8f45fa11d4db957eee530e9860ec5d469a42e90b86266bab94aa2aa8d6cc0018a5eb9f067ba2a" },
                { "is", "3b89253ec253e780b7472d324107970eae7aff161a61bf78cc1856bd941793d1b550e5c53b5eec3027224fb7f891cc8418597afaa9a850791ff414aaac9a9400" },
                { "it", "c86a031669c6fce63b0194f782f1fb6508ab84d7c1be676cc5a06b3f52ac6f420e6b6ed88f4cb00889178dc4281fe4cdc70e98b19545a384e98f6b78f97cf9aa" },
                { "ja", "2202502c88af49aa32b63ec5e1c60c9f7c13ae399f31536abeb20de1d8d805f7024b4421f3a096299dcc74e50f109a47898efde691ab28111a8eb5d48c7e5c8f" },
                { "ka", "89e9421fa02b1920b49035a3e0459c1f9661c7604fe1ba1b46959eb8352f79ac2c05019da8750ae4a0ac2a431fc321f645c2115cf77f2026170b994bd8c14f7b" },
                { "kab", "51f674ac7bcc5152dc3d00a3297219febc363413041263883d92a3ef471874fd228f13f5689743502018dcd21151faacfb4387aa0b2714ea0707d600f9cb251b" },
                { "kk", "0c631eaebeef1588c4ce4b4cae23c31362f5ad83fc977ffcc75285157cd87439e6817684e65dd2f405e9f92f309867a14aff2fa7e805bca6557042170a417141" },
                { "km", "7132648c0064e386cb61cb33350bc66c250c3857074db0e413fe8923cc907fb72f6581b177daaf95086e1569c9a8eaa6c8c62afe1635fe179a6454cac02f3f12" },
                { "kn", "2686e6d23b2cd2044705cff5a0fdade468f04e7e42c99773484ecd7356d9b542dfb7971539e00437f09d694f9c044ddef4ef1b1a62d206d9356d108285544dbd" },
                { "ko", "47dc318e4cba78031d1b8f10ce234048fa59e5f433883ffabad102cc02be2beb9d055f3691f5580a0b8f5408fd7e5a8e80d364853cbd3baff333128a0b52cdfd" },
                { "lij", "7bcc8599fe51be9c210985544452e68be42421ff8f98c0dd693b20f2615bd0670a3df2a6cf92d21434ed21ba407446728463cf94e5ff20ecff2a220f081d4ea8" },
                { "lt", "da2645d9f3175e6b3bcd373dcc1131b09507b4d02875fcba61d3fb4b77d11830d4b0eaa7c85c2e79c6ae08799c744707378cbd0266f46269e6648759b7c6f0e3" },
                { "lv", "69514856d29e23c8a53b79be61db01f85fec9dda619bbd4d604b93eb62f196428b28c954ef046d2f5d5d95584558aa8240fc05887b73f85f92bb5cb0d79ade9e" },
                { "mk", "59a334d18eeea8e20bc998bbbca3222ae31da302686fa6ef1dccd8bc90ccc720f8dfb3814d84c55210604de1a4bd00debf67f5a9d1b89380219660c105d43572" },
                { "mr", "0f844be1e7a2fbb94f65ca05fc55b0b3bffd8ee6344eef5cb7dc21291b598144bb1d059a4a45a5c32b4e0555b165726b3504866eac1e87316f86f5ff71dadf23" },
                { "ms", "f47b64c27f0f803361f791aa8d2faaafe35dab9556a23185b2c8b6e77e3ac5ad4ccbaf67a74520407a2108e9d98d5f1810747a8802cc0a877eeabfbaeb03990e" },
                { "my", "b057050c42791ec351339df49e265e5d63d6158667e9807885a5919fa6a106d433175dbaa28e2a838567183e656c6097519d37905c9d1770c4d92bfbe911a1f5" },
                { "nb-NO", "0ad74b315a08dc5475bc5a1320923394dc05d2ab51de8d0639e2d39da6763570ee93c587889f058db088b9eb7571f921ea1fb2fdab2f3243a191b5d4420615e5" },
                { "ne-NP", "63417ea730b4560ae2de67ef6b18d268dad75668ac7aaf4c306f25f6105e045cfbca6a3f4185426c296e0971c4c8b4e008b96d9ce3f26568b039a07a5961a1ac" },
                { "nl", "4d62d80ec5884a7426825b2b0931986fde8faba376505aed17262fff5308793e8d5188ddeca1e45dcbe37f92ae0c79e9d51fc21ab7003004edfed7475d80a77d" },
                { "nn-NO", "36527d2dc42815212422c89d18bf264b14f75e5a008e239534faf3280bea9d7fee56df8ffedfe37330568ae12b734a314c0109c595e358c4544f5cba60be3fa5" },
                { "oc", "2579652a9c6f973d279076e5edfeae261b0306f45212c747f0c531091d6d298753efb758883e4adc3aa6cde5f16700059790ceb31af63d5c1e9940931e97f0fb" },
                { "pa-IN", "464fe9a2c837698af5a5cce9e2a544ed309dc8a91a20feb4caa2fdf35c6fd6e959b56941742576815f7905db680636cda9bb029e7f44fbc318754b94cb19d3c6" },
                { "pl", "8944f9b1bbeb6d6f6b4f92ad5c15d0d1c51968f18c2ea821ab9719be814956e12d9757b0dbad56e25aa097584de41940628f45747822c4207b708f03dcfa518e" },
                { "pt-BR", "97ade0c8b79d7846261f110f8161651b1276b12d46f099b1296afd126734d3256703e90de74ef130321ed126ffcebdb78acf7bf6d841149e7948aa530ff709fa" },
                { "pt-PT", "e6729b038f77bb98b5829e49d81354960cbfbcafcc45a1532716b4edfeeee5f4a0fcc987d896199c4f10794ba2037fb7750e45a31d77ff1c8c85848ffd8deb5a" },
                { "rm", "ce1199cb68cb31572d9abc102219459f1ec6e51ee96e41107e08ce680b6ae1cfda70b86fc88eaf31a3eeed0e84cf4601b11367b59e1ceb4fefe7c149e4997e13" },
                { "ro", "676be4907a7e473ce02e09805cec7974c422423ee077c9ba549931e72954cbe7a27f9616cf369ebb0d1414f5b6ce17c9a5855d928a567b942ffd9540f228323b" },
                { "ru", "65676a97a26c85df7387373199794b9182c0fc3dc6fffb82c61ee5672bc21923518f4984241b13c7966e9399da8cf02e92fe7fafc5a6a061a93509decf3e48c3" },
                { "sco", "fdfaba276958b3906a1e34d64897bc26a35a0ce25fb0626a8ac82cea8ffd08d219717f87e642a2918898b67c830566497141aa7cbae21c6d126ed4794053ec78" },
                { "si", "ca5c5cefc7a9ca573482bc603a2d4082deb72b0f937701d4d37df3033b6050e34d1372ec963bf08e04512d8feee87b7bb27be899d8ca7dfda4ac3ae5f7923eb6" },
                { "sk", "df70c6aead9d2089d030cea60ed1d9c0b8b25adbd244a9ace75cdc880e240d30b8b27139094f30828eb8c2532195a735624a30f222ca3c1a80d5fedc2a6f53fc" },
                { "sl", "2843ab5dae8078cf0dfe03b436ad6504344f2a9c3a35c0d4609b0130e256c8febf5c21e377a78e82fc73ba205577dff3c1dc251a28eb2f3d3314ada5cf8a7e72" },
                { "son", "dfb9146ac1367ff0cdf542580f9c92450d4ec7fa4f5221b4d25dea181093549477db04bbecc86112d9dce1ceabfa04f8dda070df6e59ac224251d6ae8ead908e" },
                { "sq", "ad0c775d42a7c01a13972ef1d93326e56235442d1fb7193cbed4df72a71b09e4f2513cbd705ce7fc6615dad14050d386502517d350fc40d5d54bf4811dcf0f8f" },
                { "sr", "84a0c6a117a3c085b8ad432b96c911224a66cef59a480fdc14b5a8a10bcb448c0ef183d2732d6021cbfc991bec7631e070b0b6b26f8ae510cc2803fed92293b1" },
                { "sv-SE", "16e8f3ad05c8ae6205fb695f407a2a79f9689d9e756c0d0920cdc8dbf07abe26db5a901aece8d8bb737f1ec940d722b1bd8bfef770a16748e29eb239482b6ece" },
                { "szl", "06cb022f4ac61fb715da27b668866e7ffcd4cc0a2886f23c404ed4e526d0172644619f6ccfa143ca692a06f08aeed50f2e7ec4e141aaa22ddd1091409f1e8a34" },
                { "ta", "e31a9778b68d37522295fa7921ff54693ab0d2bc2160a16c66c9b481d8c030d405ffa39ceadf5dd35cc504b9fd073686099d665e2f76abe1ed78d8874e72d3af" },
                { "te", "6eff6ca88191c3ee2cdae775c70295bac391a394c2c80e6b645ed73efe5d2bddb8a945e0c24a6f6b529f7ec80bee61415ccbf7e33a725ade638b4aa3e36a5c2f" },
                { "th", "7f7c6d8f9510ec7900441b7a61df5d0f00573bfe8c7266a87a51ceac6958838a6a5d6c504a9ce264a97b7e479ecdda21e7125049edf1e4ff2d7238578e4e0227" },
                { "tl", "d7bb3f05fbfb3610680e502040587e75b3aab83d95074ad84af3f766d0d6f5803d905f9d23cd7194ab42aae5b962c2af143babcdf850c856840cb40693f83450" },
                { "tr", "5d4504c987803073f7927a413d9e4918162b1f6acc4d82a4b6e72572e9855851884d5cf2d04da71f4262eb69cb6553b6274ce843b659522a6e470d7dd4c7d22a" },
                { "trs", "77de0f89b12df693f1d6216e28d95d4bd348b1e9657ed004a20006789be8f7410198a3575a613ae1b544afae51d8c497adbac9d5418644e677a59bf06198e0a4" },
                { "uk", "50672f3b7b4d51abcc266160130a526fba4c5b823f2e0f0113731a8903735fd0f235cbaf883466280077213006dc93397d1b4259b79a7536e5ee5280ec4e9f45" },
                { "ur", "defa0c19294a074af23ab47c4d3ae8e6a21e4255500edb893833564bf698a34cb75ec67b58869ddaba5dae1e01985ab6cef4558cc78dbd1b7055f27cc9c79f5d" },
                { "uz", "205e4c5383b4a5a7a6b542f711bb1f92d282a7f2d304416a65c26b4fe5fcef6c003d34d9b0c65fbf1423db3ed06dca4d030c973640f1d2599da861962be7a8ce" },
                { "vi", "5994ea7b117d41e5cf1a1a9e16648fcc91cc60d30a4c51ed5612cbaf965d1a823da5f07d2ec27f0cfd63a38358d96a5db10d1a3cd08892d03e86a6488bc985ae" },
                { "xh", "bb857fd357d2ca1ea5d48046862316e7b5d2499fecc04668c1b21ba7148ee05235dc515bda6891fe3a02824ae282d28fd0cc7e01070e1b63bc45cb0f96915edc" },
                { "zh-CN", "d5295dd31f1c04fb4b0ae0024a9cc7aae720e792d01445eda7eee78a2837257e2a6deec6dc7e400fb333e5142698d6137c3f37ee5424d797d5f3a7ed3243bc48" },
                { "zh-TW", "555ac065299afa315c77243ec693f4025accb2743c7011a4d63c873973571aad4d2e0021f9e0f2440921a81414e17f17a767ed106aa1ad13116c73ffea576f67" }
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
            const string knownVersion = "105.0";
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Mozilla Firefox (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox ([0-9]+\\.[0-9](\\.[0-9])? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + ".exe",
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
            return new string[] { "firefox", "firefox-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                var reVersion = new Regex("[0-9]{2,3}\\.[0-9](\\.[0-9])?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;

                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/51.0.1/SHA512SUMS
             * Common lines look like
             * "02324d3a...9e53  win64/en-GB/Firefox Setup 51.0.1.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "/SHA512SUMS";
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
                logger.Warn("Exception occurred while checking for newer version of Firefox: " + ex.Message);
                return null;
            }

            // look for line with the correct language code and version for 32 bit
            var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // checksum is the first 128 characters of the match
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
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
            logger.Info("Searcing for newer version of Firefox...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // failure occurred
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
            return new List<string>();
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
