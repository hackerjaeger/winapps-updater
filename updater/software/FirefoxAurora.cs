﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "122.0b9";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b9/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "c518bfbf3010507ee60327cf8f61aea0f7b8987f76fd3232cc0723d21b4c28609725a8f736a6e64a136a97816dc6d217287fea406eb7a1bf07f32f8bbf3267a9" },
                { "af", "ff644a22830e76df943838571b1242edce9bbdb74f043a5d0e8f8e67eda1939531fb1fead31bcc25062bc9416ecd37bd4ff5993935b5422640659932075eea6c" },
                { "an", "28c005f1417b5266c090cfbfc0db6e22750ed4a04aa4ed557dbe1d601945c47427e031f79ea61b677c0e1a497ceb0546f1372205be0f27c54cdb70d553f67239" },
                { "ar", "16dc3a083ca0c5e0410c6884fb3cfd9f3de2157aaee9ab8c92b1ebf88cdc43bacb808912284562fd1fc55caaf454e9aeada59c3ca7bbac48af734af95e2f114d" },
                { "ast", "069e71e15c9275bc01d05b54d73842fef736eb5406659aac5360ec4b2e6be80f63e63a196ce52e49f3378a054bdeaa0436944aed5dfb66cfd0d658bbae3058a5" },
                { "az", "181e224f5cbd240d4d93045b83936f66ce4a204187991494b25175f21ac418575a25efb1ccefb2bb26e0e536207fe503b98a8a30a236bb3cfe5180b59e1782b6" },
                { "be", "17d7a4967b5fb377acf066f51f92514c3f551ee5c323e0f0f80d7db8e9bc7d72ec403c577ac4062ee71e9307f723806d7d41f62f57826a2e56ebd9f56cc8933b" },
                { "bg", "dcedc3d06594284d58edd7e90c01cc6c743d95e0cc378ab8825890acb2eff362e236a22e77298874a468837411362f0140314f075a8b5a96de0b2378ac3477a8" },
                { "bn", "321ce0bba83a5b33cba116c71d50a424c32cdc336833afff54692137c780dbf49037a1f381deba9631438102ac28190e404c462fbb0e49e6eabffb0cec249d9a" },
                { "br", "7684be6717d54228e0e2ca1f64e219c6d48ad76cc1f9a333360616a4d30caca44c9d46c2cc35697e72e68dac3713111833356db19a7de40017e6a386c21928b8" },
                { "bs", "1856806b24b1de500f286bf015641ff59e44c8186a4f189fb401993af5904fe2999b6b277e6ba3ac4037929711287672a978a3e60a44059d19b839144c05a58d" },
                { "ca", "57b15e218b9eb265fba515f1b4ed109ac15f2470fe27a5669eabf5548235776a741618c526ba6fbc78bcbb9094dbb55771991c449425e7a96b4e5cbdbbb043c7" },
                { "cak", "6918d3386934a5c0ca13119ebf9d9ee24a1df8daa90c822994c8a1578bcdfb859ffc6dc57185b25ef6602ce077bd08ea56d8f9c01cbbfa8e573ff23046acb799" },
                { "cs", "4c686c8768f82a1e2c32dd07e7e8832f282ab9fac46c815eefc5eeba405b29b814e0298f19d3f1ba31f71e987c134f98a7f04f737a321b102b1e0d8fd297504d" },
                { "cy", "19b643568dc5516258d6edcc62ed5f4fcf77c44370cf17a468814a850563f5bbc4634c6c673bb071c8024ea671b5732b4cf9197a2c3b627b752ec6fd53e2ed8b" },
                { "da", "0a386561239f57ad2b7cc5cccd758445565aed6040e4ab4b4bdab8642d2a6c04bea23e5c34cde366affff964b12f0a70c593cc2fafb7b2396074b9f558ca802d" },
                { "de", "a17bf929ad7bbbd350f67209cedbc500fd909df3268eff9b238dabf7264469b20886bc651d012d4b4cb159c75ca7df2cbc33fab0b979de19aec8110a241a3236" },
                { "dsb", "f2b3ace891f0c9a09472ed75759aaf10af45d1c64ff96cea6530416a45169e0eecced17b797b9b05fd986bfe9b03b4c93647fa52b73d121cce7017124dd9b982" },
                { "el", "70f8277fb21236d6990e3717f4fcaf1ff46cdb6e1dbb1859dbcb75bad4b3b7d5358c17f1b05ffa2011bf0555dbc9f409a0e55230f057836bc44633b73d60bd44" },
                { "en-CA", "22bb72d189b00d76065d23d3f1f29d9a6f76173da1806cd36a462eaaad67a2eb38db2f44e52feb65f4b83b4414c1418f93548a6e903cf5e4d556d9e6137d3a77" },
                { "en-GB", "599af04af683a740fcdff67e43a0c26256ec0e5eeb0fc33cd92bf4dd88b221aed90ccb56c41f0a18ace39b619d5267294285178bdbb511ea7841c37d2c8180e3" },
                { "en-US", "66d27535c10f21386300e3f2fc1b1a9f3dfc80a1277d50c979d2d6668ca26e3362769f052db2b922b9c5959697641f1ae6a55ff9b97a18d2ff0bb6cfb7827215" },
                { "eo", "8bf8591de7d49e65dc86ef61d6c507c9ed136adbd07c36e991d7b35c71052d70752dc54187f9bb2d37aaaec2fdca9f9e908156b915d7fd525dfad3511ae12745" },
                { "es-AR", "3949bb8ccf948e84fa61ab52c197b0da345d0ec2debdea85f15745cf9486850802943a24a4e4854d410198fd413cf9ee0517d426f528e6127d9ca24b7fda6b63" },
                { "es-CL", "4ef4cd24cda29c53b171b3f7df31ed71b97081508acd6cdc5f71c2de513b10716d089bca4562675493d4a9458ef48edb9859bf126c1039286aea0f0b1c8f0d63" },
                { "es-ES", "ac37a648ff68d074a9e9ed3af8311f71edd4ecc32822c6d5c4f932888d455c31161e6f826c24609eaa0c4454f2ec0468af3766c5dee972fb86bd27aa8dda8448" },
                { "es-MX", "7770d975fc5ce4f33e1c41325657ec0faba29d2c4fc952e4f8d76d560d5307828c410ab138f37bc4ff6f18461156cdc5ae73829388e077571cb2bc38d822c4de" },
                { "et", "03ae0cd47a3edcef7874f2d5358aa8e72d49569e04c50ca1ee5ff32dee00f01968ae9697ed66ead1318aba55b661fc00ce89e88807e86b63e61952ed048937bc" },
                { "eu", "18d67566fd9e3b99a1eb53d285d3be1edff4c76870c4011a7e59497b9f69450a22e047cde938bcab83d859288ea1caae89de8795f0d069d6c2ce149bb12e4a6d" },
                { "fa", "afad5683bbe9e464f93e29d1411f07d2712196db2feca7e469aee91e299c30c5ee90c7e7106034f81aa0a18d402ccf068d08cc01d7ced45102955dfbae1e7a64" },
                { "ff", "6f16d1a10c3627704488c068c122beccb1eeba90c5b53ee60bf40e9d0ae22b037d2fcaf0b3b5f35cfcd0f83cbcc7137ee9657318ae26fedd8396a890aaeba12b" },
                { "fi", "6aac29a42425f1e7f89f13008f5646124d6259a59052b05c3d415ab13887520298d30408603b425ca82a9542b19e034546d651b5419a2b9f9a9ec351d502a70a" },
                { "fr", "5e33b98604d4f11db25352ef2cf74c064774b040a3b5d0046511bb31e217804d70428ae994adb57ea7d8332dbf97bc0f7cf6895b8f0ea7330ae6b1b68c9b49c5" },
                { "fur", "ebb9a275238cd5e2a69f6033ec16b25b7a803e905ee3d5392e6bc4be0a1d2868344384ca95decd0057dbaef660b5a24d4d0461450529e7904deff189d42f0a4f" },
                { "fy-NL", "93725c276c77b108a094c1723bbdd64d43911e150e94fb0d15b678922d4c1d0c06682177c36ebbc25d6e9ba690e0cadac79c6d9760e54c7610cb5d888244dd67" },
                { "ga-IE", "ab9d2445a02188bc6fa698d66ce6f181028a02505c659221632ae07d590146ee82e8642c50a46fcbd6cc63e766163cb3c0cbe89f1a834cfad3e65b18ec8dd99d" },
                { "gd", "db36488a4e7fe083767c0af336d727ab214ebaf9c860252c888caa2b845f928926d5c93e29d550fe5822e0a690395ab5e92d82851efdb336b261498d505e7a36" },
                { "gl", "1e9a62e67e91e8026ba27c84f94e5cf17bcca767471fbc5ebe6df722cf7bd75e719824cb1e7b278fc50daae6f096cd8ff5a7103ce384eaa253d34b0c3a0a8732" },
                { "gn", "4c4ffd43ca426404bbc98db9286f7a4f1f6db5e5c541df4a201d1c33c9f2c7ce0c7e7c27f7c5be9208c1e5cf9674345b9b4122eeee3728f0234b75248059a34b" },
                { "gu-IN", "c3c425e7cebbd35570a9358215fc180ba5c0f8fee9db27f74aeb4ccd1e5f98b933d01837d627294d6b43b62b0f93487c703960b155810bbda31ae69f47d1efb1" },
                { "he", "d3e5af46df3ae90d5f934ae0305483bc083cb4f80ecdfbf66978042a2cd217c7bbb122bf7edf4d06abf9c096fed4387429e7bb6bd38c3ad4643a3919fd6f1cf3" },
                { "hi-IN", "c331cb9dfba57e70b646649e9e14c68f08328c236486bf1093c2f5a0e696c984ef401558625a64e1225ce268f653fa0bea2a3a1f60b819951c6e5ebf0d0c2ef8" },
                { "hr", "c0cbdf96c7047791ed4b89b9112ee4b5cb7bf59e5378cb11a0e001ee783a0502e8fc6ed5c897efd2da6f22461ca9b631fd06e9324657b2bbd766a3173f407533" },
                { "hsb", "49f8ba62dc55edf667c151b8d93948a493d5c5897b2104293e11a4820a55964e49d95e487855158ab2ae87a7fc2df8d7bd1e544bbb3f58f298019554d205cac8" },
                { "hu", "9217125021d11337681e0af479492fcb436881c9f0ad4b38d128922ebe22226016c01653beddbe59696fed340bf98de54721af909c66fa2f5888212443be0a79" },
                { "hy-AM", "dd663f1b5f57b93487b6d60c262665ee123d2fa02932b676354bb19c7813438a941230dee172264b7f59e5ccd8ba58677c1e2b20e03a69320cf8d8562c4bce33" },
                { "ia", "462c83c03145180e2593830574b866050a89f2e99583b3cb1a2af9221d634b51b0aadc80fa87df8061c25c464ed18e2baab1b82c0ecbc92df3da16db3abe8bdf" },
                { "id", "ca29deb77e830ac7cbf50b8acd603c8d1cc9fc8b201bc540674cf4dd0cbde5c8f2c7f7d10627cb3777689801e6b35050531097d230faf9bb39c146d176f72e96" },
                { "is", "944a162b4400d8686ad5c5e94af409507a19e035eacfeaca9a84d81f373381a65eea9fd34e9bfd4c0c1619cb567ffb7fa5e4aeea8e5659e4e5c99444745ff3da" },
                { "it", "8e8af8c1f78c1165320750e246f2a1662d7b1c11827335b2ef1bb96b4abdf1aeeff5555da54c090d23c33a7bde2d0393e70f1ad82d31a90640ac65e8bebd5acc" },
                { "ja", "87153bdb80e1bc4055b74e182f0d1200ea98ba7c67f28c3fdbf23cfc9fdcfc76934f7e5ebcff2a0d896990167adc7201b0c7e21d1d50c0582cf26d4c097c5c83" },
                { "ka", "54fa1911b8f3b60d8aacc94dddd02c3a0315d1100b9186848cd53ca000684a92261a8f437f45b246c59516684c7e6e99a81e633a37f6abdcfda6b5a686ea3829" },
                { "kab", "22a7a9fba1bab8354326d9c0f3faaa9cfb4e07cbe533b89fba12a42b47156594b08bcc3e71b9339afde9f00965660b1d9be0fcf7a07f5c5bf0f280f615f08889" },
                { "kk", "78cda3b7edc67fa43098ef7e3cf770159b18c5c89d7eaec660c1b306214283a6e2d250d5456ac2a5c362a893632c1e241c9921ace96503e6ee3daca707df485e" },
                { "km", "6397e26c841d821d10fb1bc61338f65e8916e9ac9314fa9e38dd3c032d0501dece25cd07a2e70c3644ca9c00465b0364ab8240529a8b1a26f971646967413847" },
                { "kn", "e8f31d1ab9bb3bf1166124a6b8d287977b60ef7997bd89b8fc00491ec58267f969189df742caa0f030304fbf408b52999c546a20fd263ef1fbca024ddb7e530c" },
                { "ko", "c5f692adda9875ce640753c97fe565cfdb789a9b1924bbe5f1730be7945d8c70366a27e436dc888d1b8bd90071c61a7da702940da7333f513d63a7575066cff1" },
                { "lij", "52f5f99e1082917c263ac7dface9094ba10fa31ce03b7210a9ab9fd28614944d775d0111ee4a25c66ed753ccc6e709e51c67e3e41df4c425022baa514c599263" },
                { "lt", "0a6821d31ef4820f71052eca272a6facb4d413cdbd3642db52459bdb7580862daeb10c800fc8501724c868a891f4ef581dc62ed4b226ef86fc132564b249d3ef" },
                { "lv", "49fe436ff2d4d17b3d2923a9952e6d0a700232e63ae8d7e00b7346d5012004d22d1214079f4399c63b3563ca4f774fa628a5fd2a67feabe18d8e4bf809097ea9" },
                { "mk", "c43b842f9c83552fe57d0e83c9b77fcef197f38d9b8c632e26707e59c6466ccae881b3fdcbc709ce696d99b7f12dd61ed5677fa7ad5bff10b69ab46c1b00564d" },
                { "mr", "201997d8111a9a30bb560eb25233974517d9cc05adad9a47d7371c8dc9b121273cab97b197c5ad94b1ea1f02a645eceb4bd2ddc11d4424572bc977c55150ec50" },
                { "ms", "667b364d094b982ec77af993391bb8d0b213d71cc62367e5ce1c808240a18eb1757816417e115214de8b880b39cc0e5f2a45eb39d14ed3cd4952b006db47ded8" },
                { "my", "a39b6bad896dba63f634360bed37789710c3f63e72be4032c67cd57c2caf488443cad98f0c5de5af9223a2a6284119363858183609bb2efac4b5cee015011cd9" },
                { "nb-NO", "196c131623bf727aded5e02f9fe2d5085d258611ec032865f32562fe18b62061101cd2fd65e5dc8ac17b3813aeca2a6fc36247cf040a46afcea6a2ee270afc90" },
                { "ne-NP", "8c9c4fa65f24e5ac83c5e654ccd9be47d720d784977c00dd206b4279d570457244ce8234cc6808cac861ae58153ec31e1cd6109ed378462a09ee388cb58ab26e" },
                { "nl", "ece6fd305f4b829c95bf5f95203c54de64275d1e96d08f505195fc34170f69c98e934bfc001bfe2c3c7c0cb89a57825fef68d2089f2967bda393335337ad356e" },
                { "nn-NO", "c5943ad6bac530ab1d19c36e6314e3c0233600ced1ed9e28c1fe9ceeb64d487d79cd1c6b28a23a6a3b0a9da2715a8dfe47b0ea18561bae5cfcf7c590fd3ed504" },
                { "oc", "032969790dde0e437764b25fc1f9f57d43c5e3e7607ecbc68b8e686fceac68dae8880d3a5bf48cdbd6a1053bc02693b8290ebc4cbaf9bfb805561da99eb82527" },
                { "pa-IN", "0fcd2fb6c229967f4cff59ef9e9ad5e1e7273fdc7597f4f62761607bf560f00f1bfd0c9cbb999710f12c96c6b925d0125cdd33c4fe93617549e13cb5291c9c9c" },
                { "pl", "22eac21e924cc3d4767bc623e869d8dd15d65001173a57983b1a43f0795b239d437e43a027b50081ca4febdbf0465252581ec47867b7919150400bb5592f822c" },
                { "pt-BR", "e89179dc2e73ab542eab397586707179f88fa79952fb4575f8a50976978df5ff5798f4b20f7423a4e163691b288bafad17934798aa77e64c687c56966c5c8129" },
                { "pt-PT", "af6d4060d2a920e3651dd15de3a38bb7d58735902c225d7adf67e9002b12714e51c9e18b7dec7724f2063598f722a417053a2f5d6704bdcac1454fa4e3a28c2e" },
                { "rm", "39dc56613eac33a15d0810cc007a90ff59322a3312062e9f3d75839a46da07943a30808a1ca6d21d42be92aa08608cff7015875fb7609c103f16bd4766f7a43f" },
                { "ro", "5f05a6a349640ca517e59f234843be851e4832ed2306f2db110f06e98436754e1029a8b10e09be162ddd17c0e4501e9df065dea11990e4ec1fa2f9760ed92430" },
                { "ru", "8b67a005452276ebdd04402176bc0194a4ab8cc34f95c40c7828ed805f6dc19bf078f7142cdf6e55f1526295298288e51bcfe5649bddf88955fa36a51f4fca21" },
                { "sat", "b2a4dfbe9fc38afd51e8c9573db2790eba174bf7ee0e62035d5cb9c3afd60cf39d75b2ab304e49e569e4eaf77c514212f810c93829516486fc43e4dcd873a2d9" },
                { "sc", "d0847594f00ae4cd8614dc8b9994c4d65220cbfa8fd56085b66d5845137c44698d911879dc8df2e885d3268187e026ec2f475b823878c68925d082630afbfb84" },
                { "sco", "6965fa82a33d472a45573a29be39c25fcd0e12d79e77ebd21ee602a771c12ae4da0a0a22bc3b7ac524a7561ac874a2473cab253b3231d374d554589c4e9f3781" },
                { "si", "b749ccf5fdcfa03e8d7096e5b0c01358404fd37af25294f4866adc1b79bffbda5ea382416619d8553b2f1418d4484c9882740b8b6adc1100ef4fa6f211e82fed" },
                { "sk", "79a0fed09a9c47618b07806acec38381b94ccf6c422771b11a3c2e9a25ecb1d5f1ebb233159bcc92943a4a6a7ef8d118b1bdd7308a18954a8be438228fa0b9dc" },
                { "sl", "667f6dec964248e7d521a0d765b4fc79abb03184fe2fbb93290e20de1d44a8f57dc36279a2c36774f467c3bfc80c98d29e79b99136355833b47618cd9f6bda41" },
                { "son", "37e10cc18dd44250590cb377778f22a75d66708715b22b964f5a6b014686460fde379ef27d28edc674d5dd2e09ec46c25cb4752225ccfcfbdbc371c87a19be7a" },
                { "sq", "19918bd238ad40d7691a8146cd87fc4387c853ed20aa5bf7dfc388a870a2f5e428cc02d6b894ae8921cdb10d19c6968e88ebcc5e7d28505f0e148963deda85a1" },
                { "sr", "0714556fdabcdecaa499fe3cf683331e37367ebeb4d047c962b1624e94101fdd7ee8f09b39ebb4ab16bdb08f4772c33b98f90562fbfe5db383e168d54cc99154" },
                { "sv-SE", "04be73810c19340d0cb0f77512d48b403187dd7e523cc3690391451edcae62da97914a27061e094aefd44b1a9f2308e0125c87efa5dc2861bc81bfdf71b4d936" },
                { "szl", "681f642ef73b15f6dc4b8fac8b7814025fb6f8af7dc84f289a2b0c67148e760dc1b3e93c1816834826e9a9151f3452ffc08654109070f995dd4c7270a5ad44e9" },
                { "ta", "d43a104fa510a152321cd98aac844cfec2bdc8fd6c2f2344104d03038500c9baf76b50049b7c4dfe255817a0d8bb97bac104e9b7acdab6359e1a281db5da6528" },
                { "te", "62dfa412fbadc7a1e743eac4e562e20c5ce120e81a971642475aabe7d0ec4f8736974bf2b45696d56664a06c472627f31b0f04fb689a7659173cddc470723162" },
                { "tg", "502547ddf0a0fcbc22911cd0834d78be18c05a2c3ae3fb8bf6b64ec1803a67546e34006ff576d16bb251d1f45e6608de72d76b568cf94b7102cd64dd9dc63d5e" },
                { "th", "d6f1e7de6f78c650831f9500be2400b8910ae0c2a9cbafe55dec93f312313ea8d4c698f17aceb61d39c25d492e2ffb1833943503f8e38e265152defbf1a0d1c5" },
                { "tl", "33fab5d1c91a3bce8e3996e77eab00142b174dcbd5f322ad621ee1cd1d46c4cb607a59dca4919296e7d3282033ed2632447644e6f5a6b34ae3dfe51d738f43f5" },
                { "tr", "b1e0d815a3e0fd5163efa071bf4402138f776dd05cccc4348a362ed091512434c6c3ef5480f16eae7bb179a1e8407766e70d9fba24311b03cd5fd260dceb6ea8" },
                { "trs", "f3ebc2fe6daf909cde1a14969584df110d7dbd2521f8f35d8a4e83f8f4e6adb6c1b22ae084b382aa3974d05ab1a273b310153b97260b3cd54ae0f5c93b1179ce" },
                { "uk", "61760ca40bd3b05ffea21d6e9465581006e4cc116037460ba7eeae7e059d715c435b2859e030856f7c58ef6f98e141aa6136413771278568b31a79c323390799" },
                { "ur", "f7b0a615238419cb169ea4a653e5632bc18121d3504f90e20bd931bdd6182ec0b8478bdbfa3eefa668292ee517dfbadafa12c0d331326a6a47b4be126032f446" },
                { "uz", "26773a3b4a059ba710712772ad4ef8ab245c112f9787062004d0060d58cdd2aebb78760284e2ccb146aef376b7816404dad7bcacdc8ea212d64516b1a49e284c" },
                { "vi", "d6129bf77f7fc18c2e61a7841208bf68fc56358c8f23bcab4943e92966ae10f3549a9e681f9f22620df01c11e7a8eac34b03e63a05a1e636833864bf86e76963" },
                { "xh", "e8a4e08684cb30eed9d89bd91a6367246df7c293b94ec8703a0a25b33fb44f4167f0afa5408f7ddd9706cb1672791c13fb9ba6f2bb4ee4ba8ae737ea819f9471" },
                { "zh-CN", "79873cde2516e44b664412ee08edf8ee683946b4c27c778e43dc874bf334979b6ffbec10835f1dcc7d9ca28f498b92ade668afc158a7b3f4fa7eb9a0cbb00f56" },
                { "zh-TW", "267a8f34dd12401bc493fc1692be2df2ff35254d026b0866ed6fe5cbb464cb8dd4943d93314004658806a9abb3164e57e3d2e7ac9d54367d6ec5b92651c82c30" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b9/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "bb623cbfa7eb87315bff01c70a20cc518a1dbfcf5ce1304feac5e2c1ca1dd07575830294e33c5deac7a7839987cabf424a8eb1bf9a33073809a7f7caa60abca3" },
                { "af", "92d38e6d137d4abe664603acd23be2a95064a6818f0234d69b6456bb82dfed736bd3ac2e94217663088f18dd155c6d82a448f3f0c879ae16ab3dec41095bf6c4" },
                { "an", "df0b91fa4251f55ce0701b50cb95a7c1ceb8eb8c60ddd2c032deb172a2612508fa2a8a41e06a2168d489cd0bd5ec197feb64e2ac44be0deb8510e543c28a3908" },
                { "ar", "965fe7116687bd592a29d3bc678c703a1c581e90d454407688f495543d1ac9ac2c24dae4575bb2ba4e74996fa23f803c76ed6d9a63757a56e5a711496f131a1b" },
                { "ast", "f7b71afc4fabb30898f51c0c49d563ecb5fb8702cc18c29a60139c20fa2dec51962082b4c7425d64b841074c2d71434b63684a32790bdb1a82b7865821e06322" },
                { "az", "751ef2ad001c97c3a21a75b6c2c85ef52e2d5eea267ce08ca2ded584c11eee6aaec5b1d8cb96961b440951aaa42cf0aba5d8c1f4b5bb04dfb28f5b6dd90640d0" },
                { "be", "67bf7daf7c1fe2105674d0d0ee7f2ddc9ca817f8f04af32b207c240f466425fe2fc2f29f0bbcf0b52bc89e42a635b8bf92b1e0837869ddbfea54886bf64a50ed" },
                { "bg", "b4e32df4b051b6c2c139ce91944ebeb720015226730ab2916981b1222226c646db09b9a584761b2449fc894f4faed9fbbfd057a86e970b4a1db2fcf57344f2f3" },
                { "bn", "7298f47d0f6d54c908fd007e8825ecde52405b93c18ac4ddf5b3ee776a88530f6545225b87bf72d8d5272883127bbd879fb53eb801461d6363d0468d2e2083d5" },
                { "br", "46d686f1d243012d45dc75ce933391820f4a0c2a09c4c1f54ae3f6881e239c76c6d11dd05b683026a9158006b654cf6ebe1dc8d5634a1dd40118b206fe81f55a" },
                { "bs", "fc38ec747d7a46aac1f2db0e1b79863fa20107f3ad67ce81fe2b1bb58b138f85e9a68ce4fd8c086349d155717986055cb0ab94ee291f8b4e5da701a6f1f336c2" },
                { "ca", "06b70a472060aa62024474045ddc8ff41297e1d1358f96836cc8615fdde5cfcea54bd9cf7848d5d455d141a74aef45a639ff74eb309ed82ed5f1e90365699aa5" },
                { "cak", "eb7e124335827b30296fb136329510fcdfac168a7b57487811588544e8228be522b73b296a4acc48b016e876e76ffdf6c6916e6ba35eac6b14a58d114c022018" },
                { "cs", "fda28ddb8e3b278cb17073c0d46e9a1c21ec1814aa057543b0cadca544690fd7d0620f10a57ed7277e8185b426a4373cdbc116f5e42214d270b7bfbc5ce55446" },
                { "cy", "5c844843001c2a23d6bc1b6cfc705ae3ee1253a067c37f9a1a793b62b5e8180bfd2a649058f99fbeea03a066415d151982a1eee2acc60608f660a349c4bac661" },
                { "da", "8a8ac68897c8d913a4d51b2f4d0b77b4b41713c9ebc00f381040eb21ea35f00c7ff7b001d02e346180541f049be49278216ebe3026b4e656373c09510eac4471" },
                { "de", "be372f7b448d7f3817696ece1f9114aa31c6a33958bb9b9d63dff15c46125ea81ff0c21c724717a32f2764cfc57e5861a7b2123b40c2a7c69382801748e8b373" },
                { "dsb", "6f98703e2d615c72bfc10d14ef91acb1494003db4534f78757df153b83eb63db013556d61cb3044351a128c20df4c0245d86a08ce26f05e64b0a0abec03543d5" },
                { "el", "8d6f225e3ad4d0d174243555d8150cc334478d0e4b4b9c48e8cf8477cbd6a2323d8ddad454891c9feb546e53ae34b5ed3aa3842522a77450381cb6abe7c4e716" },
                { "en-CA", "e7c708e7855d2572c599c6e29e2560b0536f9a80b642463e703bc36d3be36af1f5a3ccec244fe7a7a236c92bad82c638e513d8e9dc5b97f132317cf7a245e5b6" },
                { "en-GB", "56dab1a67dc13f7fb810392bac08c4dd89b31ba80e4db395f546bd80dfa20ecf520d0f386e1c9474107e38322729ed83e4711ec5ef68d0cf98909cdab1373d7a" },
                { "en-US", "a68363e8ee1496b90657c87fa4232de88af19f996d0c23efe90966bfc37941a9fd6e0c42af5a183164f5751dc37bca1cf99d02fcf690a842b446b966dd1193ce" },
                { "eo", "82d821b70823250d85f9670689295a6d5d5e82ec34b74375e670589b2a125204cda8bec1c046828cbfc342bdba5c82e4b3cbc13ac6cda0406f4f80ee8b0991fa" },
                { "es-AR", "08ea35e486a5ce1b7b6984e873ad4d107234bcf82a09a2493d1ad4ba96045219d3d923feff14b7350a76c56f7c564ccc61d0981c2ca6cdbabc5fc540d47cf3a5" },
                { "es-CL", "fa10cc56dd9f1cd8cec71fc7e67e7a682d412fe15df9b1a5aaf5b18bdfd109ea41987a7d0ff504024baf3419486bf70f6ff16a750c21f4ec57b6fb53bf2bac5e" },
                { "es-ES", "977098a2657c725b2c3420a27f7bf117d130623604718872f74d538d59b7ee4f4f8333d48093f7ecd8e41027e3cc380bb364de931f337ffd55ce04bf0b11251c" },
                { "es-MX", "d734770975eb1b3f298f1884a1a17621afcf895f1d9721c8717808bf511e77af4dd5818ce707d9c0cba2b597d709ee266a9f04e58e655a422074bb449eaceff6" },
                { "et", "c208af5768d1532b5efe125c61a37a71b55e49e4a2a8ec8b5b4a49253c51ee33c579005511810e92b2b0f2ce01420f0a1e181cd92818a7ac7d4647fca2c342a2" },
                { "eu", "50578769858068d318125c50930a55c8d8ec2590ef858a67f5452385fad7608432e9afa9a017d4ce0e2883b27438dceee705cba82cd621bf6bee5f4da680cc04" },
                { "fa", "5287fee5913ae1ec957390459cff9a80e5a2fe6387f27ecb0ba1ddce168b4839a868342be65cd15849533f17d809e820b30eb612cee9424849592070ca324663" },
                { "ff", "483441ac10191ff2c71b207d6bee6f9c2238dc48612e823dc3e46de3ec3c95ae35fcd76105d2f97c5068c40918ada542473ae95caefacd133c121cc43fcb60a4" },
                { "fi", "00e6dc23711c83463468239c505586caf8518e84d3b3864379ab38f62a94692217cbc2caff17e2b78b6ec1b95a9041120b38ffba8146b0661b44bd22a99f634c" },
                { "fr", "35a8400db3304fe16aee5d0b08e82cb207a5a26d2b4df5a26d927675b95af966bee075582966b419f774c617a29c73ad27758dd14b15276a1e89776dd10aab97" },
                { "fur", "7f6dd2c5f56815054936a47873f2c89781fa45b731f477fc8cbe4b6e1a594c786648e64d1937c50fba933a0f4d916b6b79ecb91155be93f9a604945dc6ef615c" },
                { "fy-NL", "eb6409a0a7fe7ad3e2441f0ba825eb7a6ff15dc1d1070305d6d25c95bfe3cf7f656860b1df346b2f09f57f540148c603f141415d5d2c72f012ee2213b09795e8" },
                { "ga-IE", "73dac175a52722bd00b3a52d4d63ee87b05cd99017bf0512a8a1eef3aa8ad33ba33550861885c1ff1bf001f2ead75579ba966854ab6c2b36a2163a2625113357" },
                { "gd", "9a0a44cf97858f2826a464615f18689a58a8b62f7d280b8887d9bb11ca205055f6ceee3b76f962e2acedd1fcd9d6d017854ab7255e855652d95d19075758e5fe" },
                { "gl", "c40e2fdd82189c424904933281b94b32beccb870dc16e3047841247ec5848fbc3a5550ebcdcc771404e9f99448e172edadbb5b0baffd3aefca8025ad8a65f2b2" },
                { "gn", "0f4757ae4bf1d5045be02de1227ffa735e3a9e6bf4d38f1cf5b79c49e19ef05f8336a33a59cb94b91370a111b4b7be657f46ad6a0424edc03dfc7afda70d6fce" },
                { "gu-IN", "3c929d991c65b47f90c7193368410cd9126ef68ede37322eba10eb2282c02e0314f17cc8f1f78d30ead0e8bdeebf1d17385e0e5d2386534a4c6cea431da82559" },
                { "he", "1c5e72f6e7c5b7c6e08367ae1a62ea9e9216fddcbc67eb59186f127ac024df0cb7fdbf4424fee9278329ca5aaedf8583e941a7bcc0f9914d34248830d6d82c25" },
                { "hi-IN", "f1fb23f86df4128bac597c64a3174b60a2956de2fd7ced118ba55adfee594dc79db33cdb43684d4cf8c84ee4641618e7cd3edea5ff7ba3995441680ac80b2c59" },
                { "hr", "e4b13a6491689d8a3807441a007c76c8d2ccba711328d9b669c715962cdef25325d57947175cb85d6ba3d4ef3994e661269edb2e42ab0b0c09818f4fdea71313" },
                { "hsb", "b6e15d74ef82b9d19763e74fc7ff407e523d2a07866732f350f21246a7d356ea0bd5a8fd296c013ab7c318e94c8193ac64ff39eabc01d9fbbfcf5419e6327348" },
                { "hu", "1c8ef5183866705986baa68eeab19a8dc5065848e40079d46961295a17b2eb4b83671be38a345a883335b6395e682a7dc8d3ac85fda55a541e334f948cd5e59c" },
                { "hy-AM", "b96d44614575fdc41f4e859c27a8dea1a031cb202e412f1b93ada0b2d37d3465ed9b9262b56fab67978f8ece11e3fee4a6c1c9c162d83b801d1e272a85b51ad8" },
                { "ia", "e08106dd4bb9b13709c7cdc4513b6fef6ba217629d524de6df737217e50709951b376a5ff716c4960e383b62882daa46e0d674b204342a45eff27fe9ca1339cb" },
                { "id", "874b150008f002ebe292a1de28e1edb8e8a87d004762a4356c65f75462ee439f1bb8af89bd25c1404d9adb1888ca462fa2fc544a0ab136f8d7c84b688fc1c7c6" },
                { "is", "b342d266a8ffbe4e15abf800d1adab6d80597380fe3ce159c717e792fdd23578725f08eed83679ed16faeafe7433670abce57ae1e90de3d58f155327c590ccd5" },
                { "it", "fba8bc43e851dd2cee3c811fecf919fecbfe4b9e448995aa43a5ffb7f598df806f95be304b8cf224e4247861a7a601ba039e669f2d647394978f9919104c0a48" },
                { "ja", "1789d94561c02ccce693cb2c01d7e898baaba6dcb5136e55cce91b562d0c2f80a786c641bb425c72b361a45407dc2248677297df33fda0536daad4328fc3e1f9" },
                { "ka", "9e7a59362bb031407b7469456a6d1bea6e8d0b12c991af526753f1f02b71bc64b8b14eea11290e3d3f6bf9d0c88cad2abaa388e075894eccb577976acd1596d2" },
                { "kab", "15339d5c6e1f3947256a7ff900a7a42255a96d6555c3a15f0bd2b5ddccf8a12835bd57a21bdd17e218614a5e8be97b323ba97e46c97b9ae9a7b67aa7ac4df023" },
                { "kk", "7e62f6d63db9feffeac35847f902b8d925523ae109ffa166e01d325bc61d91b018b7d67b71b82c13ce5ba183ae6777ef693b56ca91465f5cbb8ef8962c091ff3" },
                { "km", "a746d2cb01d2c3f1322bd8036d58db60394c5be8f017396d81859b80dde3b7c0ff68c55f2b769a7b6a69c17ab959656e37d41c669514d785c895fca7cd143406" },
                { "kn", "652a540aa1e39e28de0931028aac0bc6b31a60a3d7bc4d0d7dff4cbad7629bf8a978c1e661b0ada9e4cfa4b94612cce2a906e16765d7c42600f47472644b7dcb" },
                { "ko", "07bc3ae560654cc4ae2c0596b63e5725d6a2427a97aa807406389038c1d4e6c1fc5a2e1e31051533e5b1e4d737fe6286326651c58dc357c0376af20632fb3b82" },
                { "lij", "7d6eb846e9450497d5b35b69a5d9083ca53cc9ff8c2e59851fc086abc1fa2396cc6ceeb4833670bcb0454fb236b3dae2ed4cf903958eab0ff3e6d213327df4a7" },
                { "lt", "fe34829ca7cc16c6d7671bcd955965fe4af33df0b8477fddec520ccbbcd6860c36db3082cac242c1cf402b5f02c5dff3f31170dd343d0a09816b60ba71a0fb75" },
                { "lv", "9966caca73ee295588da4b856d3fb29b61fd6f2a131877bc2d384a101b57dcffc50f21ed842a7dcac8947022c8d61519a0cdbcc4886d4fcb79d14e5b9ff22f55" },
                { "mk", "053cabc8dcf7fa9dbe59af1f5b2034cd9d0e13d3d8765fae87b9a2fbb40c5c4cff0d8f3c67477bcbeb4a2324bd30f8629c17b9c4659643bb9e177c951937769a" },
                { "mr", "72ba86b1f3367a938cafb1f4c194ecda60f795bfd25852e146d03483232f3aef2f6c80d41cf981b9578e3c10621505382db638712d6a798e439a7e80493b1ece" },
                { "ms", "f636c17d7f6e9c118fdddf18083f2dbffc2db1a1c55b3bb4071ce4bd33c7022a28270b49b969babfb268655ccd017d3235c9dbe8a1ea313bc6a309df10f0e817" },
                { "my", "6c8ee5d3374ed871badf9c7bf51cbf462db0a10a5675a3649a70ca8e45c06e538824fe7a67299a5b2ffff26d4686e47bc14e9fefe486bcceea25ca61f61ad219" },
                { "nb-NO", "1c9ae906d3940dbe3cde20ff3cfd8b1300153aebd3c1b949e6933f4b2362785cc745af2b4f38e19c22ac1aebcbd7aef2c9b643e5e14e3529dd0c2434921e94fd" },
                { "ne-NP", "b6e44b0e0b083bc44baa52c5c27e0cab131cea16eb8a9720027a7d358a66e4603bc38e2ecd5dafee73722cac942aaac2d0b179e4b83d7c53e64b720ed5d4b9c4" },
                { "nl", "a23507b2621d0b1f81564302e94a106ef66265c9a57b691d3d78f8e9b60d048ff4929459e48f1f0f14d3a03b29c125d6b8cd28bf066f77b34f8327ca87050619" },
                { "nn-NO", "d772d11cb26737cf623afcac73f39e44b8f39a8c911a7b3c13874aac6bc342e0aac99a52b7356ba05a54da0742226970728449b0df97b179fef19be6dfa2aac0" },
                { "oc", "497fa97fb3754da162d8d448bdf3b58a0fdabbbdbe9a03b93d56d24225fe0e89b671e5663b6269668acc217e00dc0443688fa4675f1257ed422cd8ed38ef0669" },
                { "pa-IN", "c0f8fcc7357668be91891e503a8a430206971198488468405b3b352a5971fe2be932c90377949bd44bf75dd07fbcd4c0f7ae46ff41df7dcaa284b079ecaf94a8" },
                { "pl", "bc75a4dae30d5cda0d9a7b2f65230c9fbbc4255405b6dce6db50950ed4e218f5760e6d3ac7458add36ce34038271876f486eb40ba36959e5b88e583ab7c925f2" },
                { "pt-BR", "09eb756a26a38609aa183dc489f13156262108722cb015b9b7be251f94d5982d91ba4194fa8fcc9a450bdc2df3a62e6be98bfdbc6e294be9094c2dec3093ea34" },
                { "pt-PT", "6d8738eeaf313cb2566ee478bbb2bda090783d010271c480db53bb597a4b58dbe11854b4d32fc8d087f81f9ed9c2d3e23931e315b7d52cfaac51dacfdb74089b" },
                { "rm", "862086e25af1b0593ccabafe63ca48093f9939597ef686358cd288913baf6ad3d0d5c796ba1403ba8e36a9c8e37ee9d3b2c6a856f122eae302e3dcff264057fb" },
                { "ro", "3cd30471d6e0b3f0c79f0a7f968641362e76ed3994d28a04f2618a65f84f0d04a2de07caf42bb9085419d24a1668046acf8a8d43b76fbe7eff149b314e95f50a" },
                { "ru", "8a5788fadd452ee1c03bc503aee67028d7e0b99558243e487313ec9e8fa7c2194c93404371fee24bb0ba20b554ebb9ed9e5354f9da80db8c1bd00396c38d18dc" },
                { "sat", "05050ee2c94ad6e9dcc7372563af3b2d4a308a0e28f17c42788803c26131a32b8435cb583f3301ad2ca43c7aa98d5b0d366e6c1e7e008cff7b0c9ab322362679" },
                { "sc", "c97e254f99e6472719a17f99a359a38162b41b8ae68bc45e633fa0cc3ea94ac9b361c21a2e3b85f59e06171ad0b4747bc9d0675b91986a4fbe0a1e0d0b2e4bb0" },
                { "sco", "a8804cd146231946f4a41223e0f8e8b65d218793e53ae7025b8ec83b5b1b4d1943c728e52db7339f1da4fa918dd57e8d99ef16be67ab2c0d568043f2b38198a7" },
                { "si", "27b4a5bcb2dc31579b68dcccc076c9f3ebdd0d4ac5015c501f148db6918151a4f1b40efca638b5e4ddba1585ce6bcb1071ae9b692095e2c474c87c15fbbe3295" },
                { "sk", "2ac374f7bdfd6149992c32c7fd26864b2b325ff1942a7ec4e8e16fde1262f81d9f6e97b9a9b929ee379bf464c8d1615ebba853949baf930c09cef3af70b12253" },
                { "sl", "62d511f9efe45f393e4c03d06516dcc0c2e926cb608ecee5b00ea6719b0c2f194861fb39f1fe79afe90b1c51acaf7ab2d9744b7cc25a704f90033cfe8b7465bd" },
                { "son", "a1a65455db2e8d04c2e98d4e225a7f61ff0b5a94de363f898bbea539a0c934f31e8bd79379e792ebbc020a09d544c3af30b25aeee1df6ae908261e5189dad45a" },
                { "sq", "f8b44991a4e60f972e8218cc32690239c1b52cc407d10366a4aeea483e3a9bb418e006c9af72f1baff7c649ca613ec3f7952cff95f04e419bea01c098f323e92" },
                { "sr", "c5c38547ebf756230bf9ac1b912d68787e655fa9c1eb2a3c9c708ec59c86a9385c213ff70257d08153f942c3de162754fca9d6db5087f27147f24436f911ee66" },
                { "sv-SE", "15efb1676321f282752ae9f8db33a3df61832e9d709e29646f2ef6c3c29c0e7a0d5b2dce72c02babeed2b65df5185128a0ee35a63dc2330214d7036b95a1884a" },
                { "szl", "d4b5f00fd5db233b02d18c2bbfb4d21b8527b41fe4e18d29c53ece93723dd4ecf9b0b6924c8d29711fb5480650f84135b321ff6776ce0a3fdc2c282df0420db3" },
                { "ta", "dae154815b7acf8409c8740bcf5877596891e0c931435b2750c02e19a4777b417fe35d216fa342aa6b7af4148e247fb76312ef14f5a7459d40ad5ea07ac3dcf0" },
                { "te", "ec4a9f4089e57a62bc9c2f2148328b94625a97b05ccdce7e9c748951fcc8a4757b1cf97f2964ec69a8b73959622aedb0dd518fd294d634f77501a28a550b1d60" },
                { "tg", "1822d7c82a4648356b33ace84beba6095489bb7af48f8ec91f3642c2e1d0c8ad420986fe8036f0f61f6cb3d846136975a1312d149effc49b4eb4221727124606" },
                { "th", "334ed5a0f90f2f212fc32137e4be867202dda6b82ae064cd2d417af53cd110b7117f541ffb60afa396feb26b01c889d1a94c5a4d4a5d1f81cb3c0043208d3ac9" },
                { "tl", "6273e7d1de1f8350e8c8fa4acef724d30aab1f4153b4c845ff6b94ec3743a4beb9dcf2794fd73461d2824efa279395b5f5104e04ddab49b5640ac239155432b6" },
                { "tr", "db80ac976b1c59f14b30c717c1cf080b1b313b025515311846562bcd5fa2c6c3bbd94f3a1250d7ecdac88def66924dd30326dea1c8e27e362c5785649a365462" },
                { "trs", "f0ba3eeaeefd9b3a6342593dffd121c27106308082a139bd9f00b6bea010d4c63e352819ebb94a8b5defa32b96e7824b13c1c168a17dcb737290088680bbb0c2" },
                { "uk", "9dbef6f3bb96faa169a41feb04045114166d9d17274b7c9b558672b2adcecbdbfb187b3d2a2697f97de7d46bcf3e802231599cda604b33b213badbed3ed42d44" },
                { "ur", "801bbfbe130b984d619a1976ad1ec1e63a1930bee831821b4ab753b2317d1add33ea7619cef51a6382de27828e4e8e0df306ea179cff3ad36105f7cea8aafa00" },
                { "uz", "de2a4812f832380b377dd8aed0ed49bce5dc0f5f295cc33f01533ae3bc7c9f4a4f482608c21949c9c16c68d955ef0614b68001624c0cfaf9627b28a2e23acd72" },
                { "vi", "89235c35a6052274132c12d068141bf2b6bfc84b661ed4acdd87f98f2e108356c8b5896d7d783ea21d115004b8ea677e04e11e76d714e1126ab1e2ae06acdcaf" },
                { "xh", "210e4fe9454145b6a5b5f569413a59de1192b0c2b4c414eabd3ddb453fc81282a89d4c8075655b1f12cb51997be93f7c95a025397294d687755c698b441e03f6" },
                { "zh-CN", "aa8a52d7ac2c5229b100d4c0553f99aafc104d2f2866dddc1774cd98b8433c949ea45db367569fba57c4dfbea1a6e9640f6cf80bba7c7eada24f477e5c0b512d" },
                { "zh-TW", "33964190c602b0c575bfa9c4e7955c24986b045c02c2dda5df3bd59f0799e91cb7d86511d26f4f7fd7d0a96a532bd18530ad545e889e4fdd37f605f868195d0a" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
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
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
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
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
                    }
                }
            }
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
        /// language code for the Firefox Developer Edition version
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


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
