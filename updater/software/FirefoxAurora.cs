﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017 - 2025  Dirk Stolle

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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=San Francisco, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2027, 6, 18, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "137.0b3";


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
            // These are the checksums for Windows 32-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "dbfae86b44df55e9913ad9a9e73c629b1948f5b89436d635771a0bba5053ab7ec36b5149cdc7427ec921c9b45056fb485531218516a53c78a6bf46513a52fb6e" },
                { "af", "c5c6fb20403cd55c49bc3b2a931d641a249e75731a094f625be1426c663dd74c66aa83d03f45c5391f7c58be8646f23ad88dab8d636a4a6e1b6908943ba931a7" },
                { "an", "72690fb25f0fc63fd3a8540cff16525f002fd2ba77b3053d1135b4bb6b6e30914123fb0796a14e7359a277c6b2edb87ef4fa3f8f9acab7463d0f540f4a45b61c" },
                { "ar", "4ba1b3ef24e3d7031d9ed4a869f5255b074c7f27c4fda893940a085f4184f9c9ece42f640935ce65cd21278145189f8ecd822e15885054742f14fc35252f29d2" },
                { "ast", "d763e9d90ca7735c674516feb5d04bc38af8dffdfbe32c0066d0c6b5cd988d479ab9afb017938c8b26d5d44217e9855e0f1b6d230889affbe10ff66b9abc4ea3" },
                { "az", "765a45c582c3bf23c89922d2703edfe60d09d9c4c3f95b40cae94952fb014bf61856fd472c1a13a17fa22474b1ad41b066fc908634bf663e1c4ba2286ff53f7e" },
                { "be", "7a35befe414d3ccacb2f5cc38ce490fee0253545d1c567bed5d9ca8dcc4ba0f2ef83271589e81da9c42294fa9b918d4efe73a5c01fcdfef6c7d9fcf5314fe558" },
                { "bg", "3e16ca3faed9de2bde3a7fa1ab3ddf7963dba66e090f5c64c1f5828496832484121d8dd857e278a10d118e9feeae125e65f2d124a2e3cced3cb2bce714f9bfec" },
                { "bn", "69ed5c7d81b017dfd6e7780a21774bb3d068a496805baee8e0052b58c1904171fdfbe3b714321b89e7fd54857f8502c732ef904b1165011a74e6e0344389fdfd" },
                { "br", "26dfcbe2eb19a952ef7b14670309399beecb7ad691818a385f0d2c1ddc601c1c8d5ffe9ebb7718b85a5d4cff5ecb1f846de5dad145e97f9cff2757812219564c" },
                { "bs", "e2fbf8cdc9ce17cfc1e9e3a935119efad9485f0b6f0103580d2d9b066ceb09c685e6a003ceaf2ecd9bfb3da46ad13ad8377d5be04cdefe49721735a6c49bec89" },
                { "ca", "2b9e7291f943036a7f7e18015fcc3b8cf9ae4fb5e6bf2453d6bcfabeede1fbca47d2c160415d8e9481fce03465aa53dde8a5d11242b7476005de98e1a9aaf5bf" },
                { "cak", "44591ac498ea52b5b428e7922781aea529d5c9c6f19439f02beb02c5ea6c1db6c27d58888b6bd79fd80650c0a22d6a776f08fb99912b9bf559189e32869659e1" },
                { "cs", "29650b88372efed228208e7df47a7cb2da676d576c4ea7095f4a5d84aaf66c22954a1a3f077b365e4c6c1d0c5f58cfdb30aee16c4addfb3909887ab7e41d73a5" },
                { "cy", "28f065ffc57d6b40b84ad3054e2268f09e01902133ec80e4bae8f491c4e8b2c19a78021e66c3294cff107251cb6be407e02c22ed53196be642edf8d9cb83a51a" },
                { "da", "719007d2b8bb7a3011817a3dbfd40a423da45ba9ef6e499016e5f5e0c739a3e84e5cea93185a48c2025aa5920bf7cbfa606b7b5292497e1cab73dc0d86b8b2da" },
                { "de", "e67d8d29b180d0572962cb969e102f8c61b193cc8f8073e11d5bad8f5061182fc6515ae7f493003878c438d527db0e91964c36edc6ef46fc026bad26e1e1691f" },
                { "dsb", "b628994cca00e418d12c848aab1d64d71b472441542a49bf4fa5f3add3118de6f66817195f92923bdc2e30c007506f436d2ac22fcf2f01c84d5a28a9a5d49dbe" },
                { "el", "25eff57ff9066ae279ca54251f4d4965e59cf0b212b57019f301484b650d67ede835f42257fd3409576208f93d4c07f0271c61557b6fc003d66e1bf8c9e6265f" },
                { "en-CA", "8cf328481c90b02f834413344ff1b7e85942edcb709d7b5bd5b1df6ed5b36003e45723d6fe690d811252ba9343fc1e14a170dec31a466a1049c567a12c3062d4" },
                { "en-GB", "ea3f2bb9f5f189b05e1596ddf3a67f0ab91a0278b4612da6a773a26cd0790c3d4b71742937c652365e3272f77e78fd57931d38e5011e2e1bfebb5fe4420c1bcb" },
                { "en-US", "0426228f18e9f323e5898cea76e2b38645b8fb6f5d026e2213aaebfcc97b5dd185c8b6906b1868eec47a6664c3a2a7bbbff498ef176e233ac637e366a77b2128" },
                { "eo", "59b685968f32816f6e9a4d59e2c95d6b054dec9edf78d7265b1fb8e7d20e41442c73d63fbb12386dcaab5e7541f7e056c488dbbf6baf6d30602d4c9471d10e2e" },
                { "es-AR", "b5dbab5816769cf7255a94ed082d0536208081b78d3c8e52f18cf2e3990e411c73dd26f7d4ab84cdd21b05aeb9040c4c4ba39579ade9fc29b37474734b3b798d" },
                { "es-CL", "e1e95f733d9524863402e7a2148972b95c1bd10a555fd309957fd1a745917be0fddbdd45d7c812e372b38da54844ee91befc0cc485e421f6acd57a9f1dc391b3" },
                { "es-ES", "9bc27e12062c2d28afd62705c9222c58fc44e91904f995eaf4673edc21b5e2f9046552e59c79634bc762d4a67fdb31143a78267b9ed85b89e0ef08dad52935cc" },
                { "es-MX", "6550632cc6e447567104926fa7a0ea125361edbd8b2d2cee44f4bf7e2b05ad734aab510939a782ab951a51afe7d7d74cc8937a864d6bc5bd4d1ec8acf51743f7" },
                { "et", "dfaa7d30960481081bd7cc2f43510d4d95d72a7ec46152353ee45375127167722c38d4fc7d3060b7b1f79960221c313ccb4ff1be9f9f99682b86df87b05a022c" },
                { "eu", "10d83dff2ee9cd8c0c82cd82898189f5f410e547b53a222141ddb37591be2a3a6dfaf1b646c5170f8b4788e9b8eba21265986a4e13389194747cbaaf95d2aa10" },
                { "fa", "ee6570a8c134ac5a0d15e5c2a90b0195d3c55c01bd2c2a46d6c9891e9a4424db1d14e8a41fb5253267c45b6bcfd6f589f9e1e20c7a1ae4891f0b63282aa28ef0" },
                { "ff", "3e394e059251f8219ac2c02cb3d501bca159dd2315325753ca19b67670aff0d1971671265372a1b93334204bba43d453676313287a169ffdd1180f275fd831e4" },
                { "fi", "94a8413b1bae88b3116eca08472c8314e0c158bbaf8ef2d8c5bf185c71e2bf7bfbfe49916fb468f22fedf4425422acf23f132b052ec84e3b7b658b2af61cd3e1" },
                { "fr", "5e4b7c009673587791c911ecb030d11340777e9811740627be0c4e2628977bcc8c0395b2b82a241201c44bbe560ca29ad5830284823a6321248778a65e563ba2" },
                { "fur", "8c5af86d83aab421fb3d201e4a947bcec889b2123d599d3f9389e27d12f43a9c9984fc03e76f1cf7d7f68e8e2eefe503b8da4261e25d4333d83d59b9fdbaeeb2" },
                { "fy-NL", "a44bbafffff2403e9b2e0770096e576139c7399cd57166c5d322daf736a38e4664c619095543adce2a048861e0dbce1bca8715cd122ece2d7e793d4ad2670fa2" },
                { "ga-IE", "a1f284e7f240e1c209708fa9470f864125c3bee508d666cc2c8e7c274c391d984c7c8d751e7fbd0515bb0ddcb39d4de82d49ee465ef780e696f7ad11356aef0c" },
                { "gd", "43ce701785d51474bb67d3dd14d9f687705a17f8b791c63012ef388fd7db509de867147b15a3ca0335d3c56635e8a304a878a768718a2ab8e4f198f177fbf4c9" },
                { "gl", "3a04495be79564eda1b6efe75f4e3ee9678793fb6d6d670627bf20bc1f74dc7eea1c4340722bd31303b51d15f70b5600a3e8ac657b19f451600dd84450a0d191" },
                { "gn", "e30ea331aebc083b6602540dd75eb857dc8a5f1610c474fd1ae6957c9b3df2abc26748ed065770fd6f2891fc9d8ee0f15e2ccdbf303f8ddd7b0969c8e17b3263" },
                { "gu-IN", "f7a11b5b67b1999ece623432235cab669dd850fd211430f4412fafc7420e14862c84a6d686666cbf506dde1df7c3ce9fc78ea9cb830be5719dfff67c2c1f1df5" },
                { "he", "dda271a9aa2c7067166c658976836d1e6f454a5a5cce9851ce832e8058b5ac73a7aefbec572f94ba9bb207bbb3cf52e9d6176a4bfd1525ff0f577fb9157f1786" },
                { "hi-IN", "04f4bd01ad0f447441dd6390f9b8ee16d77ba2efc34224f4e2aa412a2a9c61535e56911cc946922fcdfaef41ffe7e84436e75c407139d3de7a67ce482d9ad2ae" },
                { "hr", "f232e9fa5f98508fa8bb0bbb16212af400788cfab82e714b56ceda8ff7bda9714b34b0d080d3d2579205cd9ade28d689384ef9c3ac9219367c0e8bf61380e8b8" },
                { "hsb", "d8d17e341a33873a556b8b302bdeb4138ea7c023f8c3e9308dabd0426003f9b41481d27a09e9867d292244f2232184a8f95ebc71148791de51bbd03f796de672" },
                { "hu", "135be5fc814d55d2cab08189170693a9c682f564c257de848c064400dc448f75aa369751df30806c462584539864e6265091e963aa99c045745b9673ec60f5b6" },
                { "hy-AM", "565b13a4f48fc25fc5ae959649407c499c965a6ca851d4c65b49da4e19ae83f8ebb0891e025ba764ca59ae67f23d0ff5de2166e5fc7f0b1912161e1419b99f95" },
                { "ia", "a2d266d2563816d2ee8c8b9d64b7834522642bd481abad7be84284f5dbffbdd797ca66e0bf1a6a259a090fbb4b5472bcdcaceecd30946aae502b5b7439964c67" },
                { "id", "8eee185c8d63824ce71cebef019c51bbf34e38b8b0c8ffc1e1726fcdff1f2e405e79df1e080057215867a57ddd2e39720692770fb75bfb113fc593d473b2ac90" },
                { "is", "1d95525ee633eb48f727bd97ff011451a966f8ce5f916408aefc846d42de3dde317045e80353d0a0e11242d6d4d7944d96f2a2dab43239e51da6094d8252d1d3" },
                { "it", "8834ffbd160558100faa01d5f1633daffd955c1d66eaec4320a167b4cd6161a1bace0a231ebb9a434def30e205a0814d15f0fccdc2b40623fa61486528a8eec7" },
                { "ja", "f799a19a7fdc69f7c8488e02e0f34b9da1a91e1be65546aef8ee579683b30397b729b15ec7d62be973e680a1ef3fd14766876dd43a2a85339d635ae9779fd172" },
                { "ka", "02bddde80fd05a503f30d32948af7d88f5fb0a6e0d921810e87f5299f6850c8aba159f0fc24433ecf625c64cfa5630d734c9eb9a264b9d6493b0c56b682ed8a9" },
                { "kab", "922093d6c95872da4ea8c5d8339d5a3dc8ba2d5eed99b8a845a3f41e1db928c93777fa8bda06e2fab3761b7b55aa6c720195282e305dc913dfe37c3e304c72d6" },
                { "kk", "4ad92a04dee8bf689ddbeb9bcaa6a7ddefe894e163b5d59207482b54e73dcc17a745a8e5a2bc06a915f5805cc92e7d486553f9c9e013d8903622e2e9f7b5ad6f" },
                { "km", "eb0fb7c63173210b1d8c59e459168cb3fbd74707b5642f558845c48aee50aa0ced9ded77c1bdb3808e4bb0692e33ccb6fabbfb5526c8b45a92db461941f6b350" },
                { "kn", "ae2736cf952161fbd1dd8c3845b06f75a6f0af756f9e7708c0007d576d5ad10b2b3d8a5433324a6e889a0d7342d96dbe76c99215773fbe0040d57ae070329948" },
                { "ko", "49d4926684e8b765df33f73dc8704dafab6ad37f74d1116ab307d7a3e796dd5fe4ed00f5b0ea316a477e96798bee96568d518013317fae87c532f2613c80b3be" },
                { "lij", "3ce9b662addd167a6543ffc64fa9d092db56827aae347ff53d7ed295c46678e7cb7bc4ad434ed846537c32d30af86da87e758fb357ff5f8a22806d18b9b188c2" },
                { "lt", "2cd68b1eb3eb3e92b0e218fe4467c6bc455d9c057495ae2dd2c87a57a2f9714ff2f4161642fa8b88a0bceee3c7542e897565344c4aec6f75f734f0e8a2942838" },
                { "lv", "c81a926fab046a55f51c9834380e7aaacf242cc758b8b487921a800e277f3e8b52a10aa340b7377b4ab4dd3b069dc9859e3d63193fd922ce1e3a30a680c7e4f4" },
                { "mk", "02049b73f17acee769a7741ae51537abd1283f713760b2fe93124ac51131842eba80a6971f62b717ddd48c69e5f636f17de731f0fc5010f5e3ac2a4d6c6e942e" },
                { "mr", "ef61a7243fffb85ecfd822f8a55aa63ff8555e3b1694280806398c89ec611d539ec0975f4a033ceb0dd22de43b08d54388b76490fe20ec2539b2914432409d17" },
                { "ms", "a52e72fa343839fefc0b3c64f5038d7e3d69585e853929e32a13f4957ae37a042362358a1f7dd707c17b4c7a5b58b3ddfcb2872aab0a3d6ba88d4bf33d0798e6" },
                { "my", "90df3e0d309f57b536c1a9ef66f925256f5c25c58cb6fd1db5f5c45a4b4b7b72a34f28dc1886debdb1e272fd7f5bf45e97409695452cf6d2b53834c77b5af101" },
                { "nb-NO", "7b93f311f69e13279eeaa4bf76939d90ef30f5c6e2548742d7848ccc457ff606be4d2d4825e62b6dac1146765aaa6d8d2ba9af261c1b401282c23c2080bffb5a" },
                { "ne-NP", "44c5bd791a4d25eb215e7a24d04e81a9b7bffb97aa49927a1eb57473a136ae1eb1a3f098779a5e74681a7fc946b736d92ebbdf184f69f0f590d9b8e80bfcf7b5" },
                { "nl", "7f6b5188dd5925a1cb2cbd87397bf897e6f86d98b22c5d853be796650581fb3c95316043c21cb0248aec92ce82b9c4f6cbac2652e637da817d3c893544d266bf" },
                { "nn-NO", "f93529574386cbdbe4476bf8e3a73f0b56486c6640beb86c0b5fd2ca59391a73918e14feec7169d00ae3b022ffe8d3b82df85a37e44612730567902ec7484424" },
                { "oc", "d3363720599acea96fa12aa4e49e45247f0048acf97da5569e5c56081263d09e4049435c5401a6d796ca3c29d028a4e22064f884da0e48cb5bc5ec6ebfcad25a" },
                { "pa-IN", "1543a18d08bf8ac10fc96adfc6196df0c05920b49664f835b891b25f7c7f01fddb3dfe7785aac08df16c017ce076152197f90fd6b8afa70bdfafad740edca73c" },
                { "pl", "db6dc4955e5fdba3d72b89c1ba6068adce1ef36149a7f9fbab625971600e10a26cd3dbd398d5f88a855cdaa00acef4200de881722ffcfe4d2326a46070855476" },
                { "pt-BR", "20275b95f16f7a254380af8a7fa8d2f8c5833f08b4d90b8b3721e14163130e91a043ae7011874d60e75c78df4c8d7475b556459930236f47331baeca38ee6f8c" },
                { "pt-PT", "644b92aae971b7182a9da73dff9e1b97f1a07fb57306fe4558a3d6a389d03b9f5dc4ce4803deadd2eb9b7b563abf9ab3b890ca1a67ff2f8c25b9641648a8300e" },
                { "rm", "7faeab6827c2d79ed3c1595308836e43abded3d57c8dd1f8917024b0567ac42a3ad77adc83144b9c6f1f09b09483c9287dc08aba74eedb66ed198f92711b9f48" },
                { "ro", "c8efd4efea250cd1bb5744162022548607fe16a2817fbcf68d4b3bf7e69f6eb8069b68e455856fcafbee1ce5a2c72a05503a3c7988819f15a3d73471c5d35eef" },
                { "ru", "5749f3df8772ed2ea444542d8a136e0aa6b3ced7d5f959415d06cd2f26cea3d3d9b7159f4929e44db27caaf9f5c19a4a0345856908a8ce3a9963836f596d5b3c" },
                { "sat", "f60eb7b7fb527fd3da0f6acef787e7a7e8ff51f3a37368b256d8a24f7b417411f3d65278fcfb337f4d7a03a80b8ae29b0d74ebb9305671a18cc5549f2cbd6cef" },
                { "sc", "9bab52b6c6479fb69bf2b556e282333b5a8213a70dfd3fa756f6a173546bc19005308eb124ed072ccac17a819d190b23732cc4e02a9517c567223e9ebd3833c1" },
                { "sco", "6f05cae9e592455f17c1ed8e8f4bcb77b0802463d0e539fe0aad3f54f3b3cd38dca1c2008f0c4bbd19c345138417e63430bbbe8e62d688c5f6e098f81f30c906" },
                { "si", "dd94202e9051558ed311f1b62af305f3effbbe15e5402c417589254071a2cc1c4e60b157bf52364fec1e6f7c23c15bd41eb9b908e8b530b6d47c466bcec0ef5b" },
                { "sk", "c6aef8929b9f9f25c5e7214cd41df6b8b736cd02ef881faf063e9a22d5eb9dc8be1e38a053c1c3997902eeaf8173891384629fc124badea27c826f161accad7d" },
                { "skr", "415520ff8eb1920415f26b2085385caa251c0d9a5aa56aca6425eb1d62f87175bc9acf3ddbc79712f9e495ee4669b96f99625b230e03496888c8a553934c7d92" },
                { "sl", "5700d9155808623313c8ec51d1d8b1b66fe54a58c324c79eefb0ec5b254e06002c1262221af36eaecddf2a05811241dc21b4396db9589f4dbdf1e544469fc309" },
                { "son", "47a6c63618cb62e9fdb823fb7002e311b5d366a78c65a7e980f38361b645cee68de83895e3703f1bdca760af0d00acb7723cf7a455bd0786ad8b533d840bf544" },
                { "sq", "cd19b102b55dc2b7af70280321030b42b30a34fc9355e44ca86193b3b3bb2659358350be8bb48393bcdcb0dbb4f3df6a2a34cfa78e03d8cae514c4d89110a601" },
                { "sr", "e04dfef7d69ddd285db1b40bf85149ecf0cee7f0b21f1c4755f09ccb96001c9c41d9df33b254de290ec2b7f6e08a5ada83a101d44b6874b2f9ff6fddc8c6e40c" },
                { "sv-SE", "ee8df1a35bb3d7da8141509c9aa06ca0c0e5552ee190fc829a327ee2bdb7cae56bf6a88c2946445079be0c6d588ad19460c4550da5b6979f89afb437bebf28c1" },
                { "szl", "10ae2f52d0e4960d528bde001c276abeb075f8bacde9d356533c15fce348842902fb811f0cfcf37f1b833316eb937b53e18717db4267e1e4c755ef6173f853d1" },
                { "ta", "2cfdec10017322172887aa82cc0551de442b540d519e0c01c633c5eb067f9dd6c2cdde2c647dac4d2079f613e5d0bfa48118839a6995584698aef9a878c8f17a" },
                { "te", "a6c9b3816a5d7251816bd5d57547790f5f073be04c506380defc351aecc907c6b4ad8a308c9b23afb53ebc3f82ce92800e632f622a310d1e93e34380f3f1cd8c" },
                { "tg", "d0241ec83125a90af5e516fff9f1e73213d94b80a2a4760c32bfac47bf0fa4e5e2c30aefe97b418853282eb693905789bcae605959bc26661daf1a57c93d8486" },
                { "th", "96839971b0bb1395601cba7e8928169739fc0f4d725f202800f9ac5632662657906a2df86319ac808a9e0115b7df6aabc352be39ee986e92c57e884ac526e1c0" },
                { "tl", "2b634ac1d5d62ec4969861e6a2a2ee1844b889375bf71c394da81528bc09b3b5371234b020f8b8548fa9c94832fe3d9ed78d7ff0f470e84d4c7e3a18f0346f17" },
                { "tr", "a4696404a4d62e82aaa5ea16863593c0f9a77ff987351876a91f978d894b45221792d3f587bdfb2c0bb281d09941bdd85b7a30f6abd5cfcfda1ebb90579b4074" },
                { "trs", "ebfb1e1e8f3fa1ebaab8685e68c17f1128064ea2ef4cfb2f8eee4544b08c543b4833f3c38869127539b90398df0cf6bbe6f5a2ff9dbd7bd87f8b350e6d86565d" },
                { "uk", "f09ea1d6b66f7857811a202c33e14a4679590ac45f989919c950e7aaefe2780db26d598d311cc425b4947486a6d56a79e864d6acb27e41f18b40e64be98de7a8" },
                { "ur", "03aa45701f7f94cd47d60791ba84c2a241e7f685445d7668d8113b1dd2a6b5ee8d5295536850092d784f403d6957680b3bbe52dc9a903cb3f7040d36e16c1b62" },
                { "uz", "cec9994a82880571b870dc3b51d6017943ae6108fe5e598623a337639ebc9e2c14b3beee6adb8b502b38579a8c93a30f15c0c67e45b1abf2aa13eeac4eedb8f7" },
                { "vi", "9a4eef43d1f5983b11b9b1abe0ce10614c7709852ab906df607c463d56b0c318d8f322ac1ff86ed374aa7ee519043113aa64d8fb1c9e6e056402e91eb33be2f7" },
                { "xh", "908f5d52b825fffc08f632515e98b3361278ec5cefd1c38adb3baec01e7a2c14662d5924d34eef621f96e1a98ee245550785a2be642099cefbcdbfb1cca2cc01" },
                { "zh-CN", "87391dc5a9cddcf038698e4ae22c5d80b398153602e49ebe857c7923087d0c8cc236f9aeb8dfdec9fa27912f5293be1628141cf358cd3bd6bcbacfd92b821f93" },
                { "zh-TW", "6bdc6dbd6e1e0242f627f5a2139b3530bc1a3ebf0501d23a453c2ac76084b45520ce757a529d21f419f904d004287c466134e09df6106278bd6cd9ea3aea17a2" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b3/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "e7955bd1ffecbf66b4ef4f69beab016c5a406d13c922d862add769c7bfa438a52aa0bb3a0dbb4e083ce322430246a2df8dc4476cdffda80f238430e0b065aa30" },
                { "af", "8d7df211dd7028af209aa35dc96be3c71c1d6e9b1f407f5b8067d98836f23647412628c57458b0cb5081b8a5f48535fa7110637c2744580783a233abfd7895a7" },
                { "an", "d1f2ecfa129db5007323cb727c5692e7e8ca6373490b0175fb23d1c97bf233e87d6fcdc1f254643e06627a0d3aa8ac8a4e534dac7a2059ee62d049d7d4d12dbe" },
                { "ar", "e6618968167ea9a5e41aa47c870fad56d8d5ff84f4f70a228bdd8bb117807a5cc1c4807dae09f2dceb6e193c2d0ac67c4a1a9a34bbb7c7dce8e0e0835081dcb5" },
                { "ast", "28d81a80e62236c8cd5d0e4d154cd98a1d7a625e18c7a5688161e8f49d0645ca280e5fdd5c01d67c05662e0965bf5bafefa47f685c665391973bffe7fa66eb91" },
                { "az", "4ad36564e722c33b8f7b2763ca7b4c00fe64623bbac357abb7b283c4f225a26e42ac0870be0aff1bac168ae8967a67e98e6a4430f0ee43a431a2a50e84f66ccc" },
                { "be", "651a64e7566e5b3e7cc8db9c37d3cef5985654785a401d3be7a59d77b8d652d89f6f4db23039601b2ef961ff6dd79736ef4c47f2cf15de4a8e3fe001e0717247" },
                { "bg", "400c9a7832893d2c11f2347cc69c3d8aee215508289e2ceac946fab26418d0e03324117f623bfe31c93e066120eee5fd14008cc7eb0df87a3c3b881d9f171f7a" },
                { "bn", "062f19ffc7d55d7c7f2e81b8f4fb5086932efb9343fdecbfc49a51e4f034a23533c7f186c732b328918a68f53498d5d038becbec9ccc8004f178167d1702ad35" },
                { "br", "5fdea9acda8ad128167dcf1bd55871fca93a6ca7ab1dccb2e76d454c315877175368f0cc1f271746907589fa6bbf765397d31ac04dc1e399c5cc3f7bc154ac13" },
                { "bs", "17ca26d1e30bdcac4a1bc08366ae285056be81886ea2e1f81fb508da4eace8fe5139e312acd94f762ccad14fdd2247caf0ea769aea1c36a53db0fff05eb8518e" },
                { "ca", "11d9b480f65a749a23dea09c471dc5faaa04a014cea14feea645635feda1397ff05106aabd3192a351d011bbe51083584e02a3eede8c8cce858165282667b42d" },
                { "cak", "568d77a9d554d6a5339c6f85d64920fc79b250c7d0e6a254bab68f6e2dfb50b99db900e509969bd399ed51abd837c5829210287d2bf1e5b18ab5898e615a6eb1" },
                { "cs", "1e9377b5431fbfc5a71e4c2e8fc8f8802315d33f083865565a841dc6acdca9ec8726a55b4735ff2f244d71da0b3cc0a83c6775112fd141a568323961f5618207" },
                { "cy", "e85b7f9b0aba574c23ef13fce816b6f235aaff70ed0e3de7c128b9fc49b85e8797ac558cfb0f9e6fcc77001b9f8d9a7c56b947649dfd8d2143926e3b37318846" },
                { "da", "a3d85d65dd405f38f2dc0b6176a3937925f33282ddc6e7d78115ca2a34ab1c192496f69178a5c40b75d60fa00a69a43b64c0661608918b5698bb3533508ff7db" },
                { "de", "6e3a7ee9cb1a8a182f7d0e622a8f0816c2bdba45131b2e24bb93222d3e72c0a7725e42b45bf69752ee446d72639e9641f18fde957a74e7ea41d21d52e9b76599" },
                { "dsb", "a0be0bc06a8de67d6ca4e2149a0e16663750e69273a4c556fd5cb2d023f89e97947eac1bcee42e3238f25ac9993d82ad8f0e10522182d9c507dbf57eff4c8499" },
                { "el", "4e8cbd6808f461425923a7d6ccc743d5c7d0b043e3892999298fe267c073cbb41099b62096977655fdec63a93fa941562352d89d218c1dec9fe33682ba8d66ff" },
                { "en-CA", "3655071c16b6c0891f0db5b37b95cc980b6c7115ec141948647b6152a3b5740623726507657c5c31e2028be7cb9782380bfb23d5577fc0226a8cb9797ee4d36f" },
                { "en-GB", "24e5b89cd645c13f1fbf8d4dc83687249f9a96e0f487ae86e0ff130daa8947273575d7f84c61115128e75c5146fa086063a7bff3dd4f3e0982e72cacc3c7aa4c" },
                { "en-US", "759ed275d43a29829c91b5e088ceba85ead4d1c2d570e20d407863a3da0394954ab89a9e0985b7d26ff5c2308f7636551f9e1e001e840b79cb9c87b15590a17b" },
                { "eo", "532c72f630c8bc520c419dbce7a679219252d31113653ef88592d0493d5bd09ee1be2236f6c313958718182d24ff7a0abc01653db69dbd3f667df9eeb221436a" },
                { "es-AR", "019b34110dc7be1a560f67ae24101b1b365f9857fe4d2310bd8e4766d2d3d003342370153760048bb0331e2262434885078216206c7c54db2dc3cfe2491e00ce" },
                { "es-CL", "b0f3f342fb208efec3bd23954dc81fea07d1fae43a55916a757223abbaa1e2b314d6e7ec6b18da8c62e2e116f1c8279366863e6540c3f873aee1d5499d91f2dd" },
                { "es-ES", "33e3986ac4fefc6d2a3dbdeba477c05fef9718c0cefe31dd89e0caf72428facea31877e6b82dc3179db116a4e045baff6b15eb4d7b942222f0f113cf8a55b5c7" },
                { "es-MX", "8b8fb8ddb96f224bed02e8d701c3486520d6b6c999605f702281e75e586e6ebaa8716feb6d5be05ab254678f6ce4eae70a9a62adb057472803581ef55b144598" },
                { "et", "abc1d5c7bd1588643c7c98bc94f3e37a70dfa9e2d666794e80da545eb75171c2573c61d2d149cdd81e8dd506040bd2a9f84763a9d54db625eb66ec78c0a780bb" },
                { "eu", "5f1fc46d12ce068d73820193af2936145659850e7a98011d9f5f0f1f251b8ed41a6757ea5c7a8585aaaf65091aaa09287956ae2c1902bae795b6be7c83389200" },
                { "fa", "4d9fb43170814f8118e3ba551be36dc8b8ffa43524f14eb1315f0ae07ee66dd3d9df2382341a7d6d3890e51f99896560393bf3ecc00888b73e307cb5ee7149ff" },
                { "ff", "59e0c4a9452b73a9bd58d07c78ec12c956279184453172f7e11bc81d7c444e61ae490943258b56ab0a84423fdd16f1a0146fd2f5dc8357179f6e0d7166860db1" },
                { "fi", "a071963ff34c43a65b00a85b43ca56b3b2a7ea83c1b9dc5b7ebab4c30aa4ef1e5ca732e312408d314b4f0bbea6e6090f2411f0f999d20ec80d776fc1ab62891b" },
                { "fr", "8a66fbaf4989a9036ca641bdbd73b7d1aac0925d4d4edffbc0395434d31a3063b41d256d4ad9c9ce24848879a9c37bad28e528231012df94643f87ee926549e6" },
                { "fur", "f46e106f4904b2c6e1f66e405351612be87a1638d5a233fd67fa94ee64ef773d2ed087a5df8e6471d849c4e9155eb44657beace85c673bd18d9d8f13554217ea" },
                { "fy-NL", "0b31e82fdb3064913e56f77a531f46d0df2b9b955b7a0d50a2609154d3b3419f8bf0710fa108776bdd0bb3eb1d3dd58834b04e112faa92f79a6a412c01cc6399" },
                { "ga-IE", "0dd2d4c4d7c162c26f9d97e9a22d93a3d691c58e6aaa73bb6bdda038256c7b6565eb733e70c3ad3b66a025a02c89a40905c5a556b32d0fd2c9b79131d3d728f0" },
                { "gd", "f7ddf6655445a8b1a8daf1f9355092a48679685f8663428c66190813351fe0da1ae79e816252b228fb66afcf1995d80a7b39c348439960e88ef1abcfa37b5944" },
                { "gl", "65254bc9dc2c45e57d5717f6b21dc9dd8370fb550447721c2b43506b7c6fe6735e65fd2b8d6eddb8c0948b7b992e93b1984df3f99050173763d0893c7a1a4e15" },
                { "gn", "53109b77f7ba10696ab8f2335adab316c50326ae0afe6e8f6b2fc77bc4b855a37b9fc5e94fbc8adb965824c4d9b220db4339ddb5d621ebfb4d8dbee2d4ca89ad" },
                { "gu-IN", "051d8ad937d6e8b949b934f1435e8cbe118de7f35639b00862dac34d325994373d69be62e0a5080029184a69275755c2d89b4585919ef42d2660aa9ff79f1f00" },
                { "he", "0f2dc92e22768715a961c9cb4b09879ec23e87560ae960658ea8319fbbe7d1d4006e80c40fcb388ce947ebd79067ee884d7808f998ba9f59cfa78322cd0414a8" },
                { "hi-IN", "f7544d210e377e62feb1503d1903bfb18f146e01f0a30e2aef2f2bce1bf7d0b7728299ce0629ad101306336f316437234b699665ea47f4f9e000157d2f952a9e" },
                { "hr", "0b099892d8283dc9ed43eb40fa13e22e3258a2712eac18a5a24e2cb17c15b58900f52a2efb2b3104e90f4726a3cc8e66234007936559c31a9428a2fd3ad5898a" },
                { "hsb", "a1c531cbd02e8723858411db50e03d3794f307699531587c4bbd3e0d1745b312ff6f3a2032950563e82e2309328d5df34f702caa9c488ff12b895a60413bfea3" },
                { "hu", "095d5c959b5c913e3da3da7f7f3df548459b0876c676bda2a21e3fae9c8858349b0d9fcbb99c035b304539cfe4fd69e28da550952aff8b9ce777ef9cb415b96f" },
                { "hy-AM", "66607a1a36cfb6b94e7a0533988fe04362631490b73aa09afde87db00625cafb5ba362da81f0aac50d91ebb2e222b1e834794da1d73debf935d42a9dd7c13f05" },
                { "ia", "288e571bc04022d17b4b38bd35eeb6a5b9a936678decfec57d3ec498b3ccfeb77828b74813e498f9c69eb5fa0364dbdd5404f11744c39acad97e7aea0df05324" },
                { "id", "0c9ba75e28cfeb71b837c3242778b5079918cc508854e8ac3997a4834575047a13a40b64a538a561e239b475a43a2d1c84a30a91ec22a7c753b3a59cadc7893c" },
                { "is", "fa96a1188196e78bd07e277d40ed15fbc3855c866fbd6e8cc85efe71424659da822e5abdfaf8d0dcd0f2638e246b94270c9dc3e88d37cd324afab0abbaca8449" },
                { "it", "90c68619b47f6799ae5f3e4a92d3315ef00b79f6d8bfa086faa4ad5ac8e302d933c270bc20255e3622750188ca8db3da0c224041238eb6e9eb686a8e72215be7" },
                { "ja", "1f9e8370727d714ab6110db485a2bb2913ed6a9d781e675857b51547bcf0093cc9207b298762e5279a7fbc8f6560e6e3440dccce50a5dd6b172f4e1b9996ce61" },
                { "ka", "149ab2d23c0ee130ebddabb69822e864c73e039766c9aabdbf8bf50db7c55c147eca2632d3d0533b63e3e102988a8c4fc0f34c8ee9ec81e21c5d99529968c60b" },
                { "kab", "ca46034d9bbfaa2650cc139c81efed88c835200e0ad0c517851ea36a1a690d0fc8921bf9545a5d5ccfe52588088316f61ba279ccedbfcab7ac98baea18f5879a" },
                { "kk", "a7b6718d316108e8b45286160e553719aab273801fa9c5c769f02d2b9a0a2980124c05f427ee2ae6063a5d3b42bdcfcd150fa90d97adb2902c56ab73a6854e63" },
                { "km", "d756a2db89928a0805beebe85b37322a9bb7c9ab4027a825f701ebfec510ae3b23efae546ba681d8f72326fd7f6258408e0fac0ce8f6bc376b4aefd900d13c9d" },
                { "kn", "c569d672ed3f9146ee65ec84ca05ff846e63b9bfb2157ceb3a3b660bdc3eeb95582a2149e41692b712ecf29e47f45758550084c0cba6d528fda2b9f689675e1a" },
                { "ko", "7b94a90f92d57882a255607685ae25bc48bbc55270a056c7a531d6a108755dccf507eb239e6d9b48715df6610f3540e63005514e5e69da68df725bd52b291cea" },
                { "lij", "26adcbb10b26435a113c1fd6f61a75bf5c04ad27f305b2b0eee8a7c425e29eb21874f8af36bf07d167788cbf051a64b5f7b8a7e59a6054c1d7b8d4a04e90ae99" },
                { "lt", "ee3c359aa852c20f6c3f2755ae13327c64b060de9d96eb7dc97a2670fe15f0803e7333378c2fd7327f06c53c6495073ac7677abf8bb308384fe05e9b1bb680cc" },
                { "lv", "415c619f77af0a1c490320686ee17cbd30f6cb494152eafeddb8459a32f9f03b523e95a6520f97b0a3e5c26e5d44bbfc49e55971241028acfaf0da34efbe6fbc" },
                { "mk", "cb5f0ecb78a333bb06ee782084106681d32a79c9def1c3770098f31d7c215e72f7c104008112ac33180fd8cda8b873fbbd43334e6452655aee040c7285c21383" },
                { "mr", "28e0dac4a0fec7ad280de4d6a26ec38d1bcb49e6637b0dd58f58900c0ff67fa9169518101384ef183de48063b2c1da4dc844b495ac9af1eead4a96ec4d55beaa" },
                { "ms", "f12cc447e6368b74864e34c1686d1f1f159ccde182006199b9b6ca4a07b340f5c85fea0ecb1f0bb29e8c9cff41f2cdb1f3cd23cba070552068a32319ea63b5b1" },
                { "my", "75d762139799135e3aca452cce7ab4ff703cf2395b2cf945c93fd87f38efc7672eea76930c1b041354e2e31e87f42a702333597f44d6d4e93e88d29346987efc" },
                { "nb-NO", "d045a0d307f48d153c48c54d6e01b2ceafcdc786489d801fb87e363b929b0b92d88dca03f9980f993d6f9f1cc0d1965d67c3fbdeee940055f8a98a9110557f16" },
                { "ne-NP", "3bc0e226e0119b878ee1f0763e688891023809ca5d47050204c4faec4648bd8cd01f7e3282587771d08f5076f6e369b8737ea6cff4fcc957e34683ba3c12757a" },
                { "nl", "677ad4b42e48154b47d447f135c46b1675377151226336b698edbccbd66734861dc1a2376cbc926339ebfd7e588c0b21af382376b55651f8fb11be793db82723" },
                { "nn-NO", "ad9db44e121465d200a20c237a52205c26567c0cacdfaf67fa6865833f2d5e86e599dfe8a8fbaf7d9c200ffc5323f5581919359a2654fbad2c506529f0d86cdd" },
                { "oc", "cc1ac616450e919e3aec40dfe6cd69768aa43af2a48d2bbabe53cd3687dd9399b03f99db0fdc50ad69dbb7adc0c9caf2cd7b9d62dbec2fbd0109a874c306a534" },
                { "pa-IN", "529e03824c2aeab6d41dbdd5dba3d8804d35648d5493360e1509ca6406a9f6aab76ea5e6692d7c949a0653a00b508c062260d5ced4249d6c04bae2d1d420d7ab" },
                { "pl", "24ae07b8ebd4205187eb1c5137e6580d1191c1824c2ef2b136e1af59410632a4e6af6cc34d3425390e66a3c3c62de5acc24240cce97a377d08690069790b8a8c" },
                { "pt-BR", "31a5481cb97a9bc0d00a3674aa849181f262884e1d00b07da1cf2b6d7186580d50bec8afb0757a16e08ffc63672ae0ab42b48e9495b56724c1e2002a3a7e180b" },
                { "pt-PT", "ca8f4c0139b54d843873ab62f142a4b39f8f8dac18499f4d00a741a70aecddb1245fcb7b385c6d7aeaa6be66ca4fc4bd8c60a4f2bed8fc5d4cc4ea0414d966fa" },
                { "rm", "32c6aaedeb64224faa4cb04bc44dc67db010121c37d6c789b163bb08c355e2b4c14fbe3da3611a8d16b489edba86be91a6c30df4a45e038ed67f68376c2e9b25" },
                { "ro", "807c93986536875a2ff11b3b7d8353a805e78abf3f9a7e118b9c84f9f6c8c3e90834c580e5f99131ac5391772a7da7de81aac2db62606c26d71286754444774c" },
                { "ru", "7ec9c9ca00392035d579afc79866a5b45a497a4aa0bee68a08f14206abdb515ffbbe6c5b806fb8cc1d491ed901105861b70ee76e6ac53e5469654d78938e2e05" },
                { "sat", "c73b63a455d1f3086e7b47b97f8efa858bb92120fbe11077210c1017aa607ba8c64fa259a70051a336d714e71e351ec52695b9fe9a3620dc390d1066c2ccbecc" },
                { "sc", "e5e63040cf90ac1795070c7520083dd5d24aa1575a748deffe76818d713f1bbf80273b4f2121e628fde18776a715b1f26e7c4a0be4b3fbdb8acfeefd8fae05db" },
                { "sco", "1320bdf86b08aecfb1e09fc6c61ee1045f5f430d11625dd9a65e218289e42203829dc4d7c1557e2e517b0ca52aef005715af33923fcec25f296cec495aeeb583" },
                { "si", "fd54c7957fd438cf82532a67ee3441270d04ee9b529820daca36762d41538380bda4b8d023ff9ca4051c7eb73bc45842416ff454860b2d1dd61474bb80e3abfc" },
                { "sk", "bd932557a37273e8fc7a2dfd03f2cb5284c4b7dc7d5eb710030757b27ffd2d1315a062d5a8af9438362bc4f27437c8171aa1166e235e78ddac7897413af18a5c" },
                { "skr", "2180e501cfaeb1068ad7d05a3ae6d743676de07a9cf2410cc6bcf5538087ab42fc8e243a642b3cbad88ebda0855e59f79e7385105f584d54a4afac9f31a7898d" },
                { "sl", "3ed8fa2693b01eb3bd65ff38f8500f828796823e6886826d78b7c34db37f05a2b8b6a80d4ac49599a652a4fb9f286cf8a3253ba8192572b749ab0dc991c3b384" },
                { "son", "46e86fc78e862d95e9fd18ee53f3361eab33b79d9f2bbe8f89f1b85817caa91cb0cdb0d8e95336f36742b926a97c788b140d07483b49389e75c6c2c26851a41c" },
                { "sq", "fd7afa2a6080386f87c44f0ba8316ab0b3976ef76e964f33675884aaa005ab7441b9d788c05a34c88896c1abd109606c5fa66d9cfff4e22b85304915939c3626" },
                { "sr", "2650d60ae9f2211ab4a258c7307e33acdedbeda37b9abf6e325e648a6cc90f4e8fffce71bcdfa006fb6a75938a0af90d8ff67978d94a94d9f5f1f60914f1bfac" },
                { "sv-SE", "9405578ed55466667ec3ee38e6a92f9f1e204b2a0d2587633934439e6efb2a9e5d772cbd887e1617bf7174517ec2b42e5d33bd0430e79a89c81cf0e617026cb7" },
                { "szl", "0c6fca54bafbdd2c6c03cfbedf5f9eca9f3ea100de1df724f4002cb0ad72786b257d3dc27495a5655e4bcc59720db73ad00c758ada580ec461f6f11dc488928b" },
                { "ta", "d749ef9a4f9fa05f63af4ab891a0f6d63f213967607e0bbdc72ea2e67626329faeaf657bd7f53714a208b0f98c0399a04fdce7a31a43cd9ff8def340670eb610" },
                { "te", "28e47fc2b11956db9ff16a57900e9f6506574c2a50bff2397960b8b990054d56f07aedb3f68699396c6006173f8beb1012b21b237d0087e6af515a6854ef6137" },
                { "tg", "2432e636814589e18c82260904cfe7298d5250f8b93a304a1069666f8ce164fcb3ff80f2aa79beca3bcd125f8d3cbd3efffe2ade607f3b87353c2e0eb3714afb" },
                { "th", "b4cdb0f635b92cdb2e644f8593b2d4e575b6c443c4e93cc5dfcf1c8b055c7bb54bce2e9adebcf8b04e4a1ac8e068fcf359816b1f0e340a9af5ced17efb30b77e" },
                { "tl", "b7ecac46b0fe030152e9b8e338635081ad147dd9049646c76c2332d962c2199a331fea3eb32b272c30c17e29afab9596c3fa098d1e467a64b57d25cdbe2d371c" },
                { "tr", "ee111d137cb62876b6ecde35ebcd89f413fdfd8b2cf0a0820747026e62638f188d81ba7a2b94a98fe0fd6e1e8fb30cffcabc2ffef86c68de93baa52b4ad6d2bd" },
                { "trs", "934f0abe8a742f3fc796224389e06e9363c9a4f81710ffe6945c7710e72a0ff3ce2640ee98a34d3e7937590585c80a0780cf4c74862b778e0fec9ba1c11017e9" },
                { "uk", "183389f3f74c493bf3b83f6b6bfd7a7e1f3ba62a66d9b600a52d94a4e8742de05ef0130f8958a34048598fd9fd0a6c73dd7ede83e9ff5055b1af7fe2916478d6" },
                { "ur", "103a3b1a6d003a5b6a5a05412c7c7a2ca96e8e8ff0d373a037e6d3598ed149ffc2e6e38eeca3286f40331fb93061ed91866ae633928803dbd80b8974247d1d49" },
                { "uz", "341714044afe31cb04441f986c87b5280d1552867857e3a8ec43b14520c7cfc89dd7a0d7abcd72cc9c79aa166e90051e93602e62a5a72a5fafde9fb85e0fbe22" },
                { "vi", "a7f175aef36cb15dbbb36d6054fd4b2190a7d39d0710d8ffe4b80a6e22a526f4eba1a178e947746f51ca102c295ecbaa875293981e59e81d93052ccdf0f37682" },
                { "xh", "09b88b46685bea013eb34dcf46943d7cf4dc276a7db5785f938a987facf063c28e900805ddf456547c2e9a93c54b49a8ec54ed05f1d0e0cb4621fdb37a4f2f05" },
                { "zh-CN", "d8c945de9a551a648277ed4078d5560097123c4a08f6aa9690619784be8fa411fd19a8b61e804d9ff1d791b3333df6f9e92b0aa9e712061a5fc39362d5f4f102" },
                { "zh-TW", "ac5b090229300b87b5f1c20f95973926b194ecc356a30884805d779d6aad3fa056a1818e0ad916fd4ef8bb5e0b38a252eb23751025c49fb5fad99f311390a40d" }
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
                // 32-bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64-bit installer
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
            return ["firefox-aurora", "firefox-aurora-" + languageCode.ToLower()];
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
                return versions[^1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32-bit and 64-bit (in that order), if successful.
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
                if (cs64 != null && cs32 != null
                    && cs32.TryGetValue(languageCode, out string hash32)
                    && cs64.TryGetValue(languageCode, out string hash64))
                {
                    return [hash32, hash64];
                }
            }
            var sums = new List<string>(2);
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
            return [.. sums];
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
                    // look for lines with language code and version for 32-bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = [];
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64-bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = [];
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
        /// Determines whether the method searchForNewer() is implemented.
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
            return [];
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32-bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64-bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32-bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64-bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
