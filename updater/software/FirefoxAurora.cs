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
using System.Linq;
using System.Net;
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
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "97.0b7";

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
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/97.0b7/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "d87aa13c600c89e3625c49a637fa693c3487f3e0f6b22b9db1a5d352a20a118de8ce3d3302008effeb518dfd6dcc0597d989533d79e80cf61d9aecabd4f00b52" },
                { "af", "5e2718eee3c65990e29b17ad21b4cc99d5029f82c2a2e33cd9a5e6cc14176d7c66facc1c3286928821e3658851c61ef9cf1d7e94757157db61bf12e73fefce85" },
                { "an", "a63954d6a3b9a2aa3ef25d73cead237377debce6ea4aafd2dafb852dbbf49c4f26e87a07801233b928885dbb76761357cbe937adabd1f694d4cfd107aa44db78" },
                { "ar", "84d3c445b46db8d283c5129fd624e00aefde9720f2075a0c89060b74a14f7bbaef013645c12b594730cb7153316345f6e841349f109f47b72f5d87ab27921965" },
                { "ast", "2901001da89e686561b68975c145c54331c2431c0865bfcdfe4c8ef03006723f95e28dd7f78d9bfdc77fa143b465ad8639e8e0341ecc8db9a72f32fd57215393" },
                { "az", "07f0b004af641052b8fd1cbb185919740f225b547b1b3b91c02b3d91ae6bff3b87da64b49b853635e398e52187a0287f351346e2a9f4ea51c4c7d652ae688c18" },
                { "be", "b028a99c5e1979bdcb3bacec2a8f43b3bf3f6a811bcdb663af59955bd87283a2090f6e2f6a6f5457819527ca393867b2f9f096cc8ee6a6622c35171caa4f3277" },
                { "bg", "71d2c9742c715d812f505082dbd818885237459892b5c1b4d557475d293dcb8441565548864d9f40f1b57feb8b0e792222437fa115d88d15169cccaaafe80fa2" },
                { "bn", "1e3dc40ed4493c7bd7546abf0d2c418acbe4c5de1dd34ed7286d90bbb64d89f557861fbb314329644e954643eefaf7e769c65fbc7fef744bea5a96c8ea5795a5" },
                { "br", "ef091e5628112c7173a54e6d1e0581b0bc2fe163fc2bb379ff253a56fc128d59f67b78026bd661cde920abcb07e35f46cb8f13ff39aa3609e389e66aa59e586d" },
                { "bs", "5cde20dff64b2cb9e4db6bf3fa1af1a52996ff53d4e98d277860adf759aaa2c3f279477fa7f1d32897a5ab2773604f88b90d6b7f3d0c6a1f6d5d39eab81134f9" },
                { "ca", "ec31e966f2f73a2f2e3f7e9060dad53fcbfe69999f6bbfa34b1ef585fa26fa2229daa42bc586bef49621583caa418e3072f76db745fbba0e16cdb41d1f4061d8" },
                { "cak", "37b633f374b690691ebc1404fa5d77e6876011506b92c3c2abacc20a3bc5a3f97cdca700a542a1f95d88f96f203c548175eebf93ae2f018e890ed6e24eb4521d" },
                { "cs", "af0ce8513d59fc3e13530b069dd88ab97ff954e0f034459635877c9925f097853f0075164bd669ee1e715254e0aa646f80f506a43ade1e1207da4923b495a82c" },
                { "cy", "250a8389d5360d4440013e3ecf5365bfd1c670574b06a64572f5bf26433ca00b67c9ad0c347963500b07cffd74f2a528e65b0d1a9721333319d6e028c3cd15b2" },
                { "da", "1e305878829ca99b652156d1b2dcb0fd190a2363b27fcf6b112cc910326dd0fa8af9d9d4eb5e1772b58ea348634ed8903189742b59e609c69d2967c0d38fe441" },
                { "de", "20d969cce0b01c3929f8177696474d21435ac4caeb250f59e901cdf1e4b47634259eaab5d2c34f703a4821d3215ab15e170b141f29ebedcc81e6b170b85472b1" },
                { "dsb", "6d98767a96fb692515a0ac5f3be2399ce93e1fee71f2ef25fd654ebf877440ed31755dc6a5cfa8c0424ada136cd7f5518524b736082d7614f0738ec3d51b158c" },
                { "el", "2833d9620f1be5fdde73fe03f91684a4e3c05ea61f8af54af75707c31e5506c278724bdf49a6456cb5cb05fca06514b168a0678385e4c78bc33f835eba6c5cb3" },
                { "en-CA", "037d7b1fbe887f5dcaa402f27c267c42c1fff01c04353d0f04408d650df1561351e68530d922eacce48cff17084a239178a2d0b7e444ce4c33b950fbd2a52d75" },
                { "en-GB", "ba0147b0ca29ade3b0f5a72b6ed819df843d146305b72aa7fb34bc12550b68b64cd678a5ecaecfcbee420ede85b700aeacd34bab0e14e6a4862ca0e3b4ce4828" },
                { "en-US", "9d75ad34ac4c4bfe6f4118616d62a9217a47e586db264815e5783fdb4822e7358436985cb468210e1e37510d40754f73d353ad8cb70bc3d67ab06bfb1a0777b1" },
                { "eo", "9cb846f59789ca80acf3fbfcd8c6423bc8ae635216694527d74daff719a4b428472641864f51fbde55af3b5c78098a21254f32bf5f3122ef8c26699aa61bf722" },
                { "es-AR", "b1d46f71850ed1ef6b826cf55ac79693c32e5acd1c79bcd6625339fc731627965a45e03b4f700dcbe4ec50ceb38c3cf6e792375951f0a9b829d97bf7672c5f05" },
                { "es-CL", "f6f4c6ae3dbcb4440fdfa700499ec766ab95cf4946a3e4e774d126ea07faa8d1bd1a1384e9461ab6de6c61c5f75a7c97a9fe6aa9c133fcd3c813dbba81c0d848" },
                { "es-ES", "9709a8b68771090bf55800e4b2f0bf5d192b25348c48a77719d0bd696020529a985191b941eb8fbcd3e951fada3a027218eef5803687f7e5618f98ba61b8e1b2" },
                { "es-MX", "2865876b862eb12b860cc891749490e59866ec89119a6e1c290c0a2c5235ac4cc5655e4cf3bc9c91e3168cc29ef069ef88c75a57d1db4cee2b5db9f8e2033dc5" },
                { "et", "22505be046047026dd4dac2502c3b714eebf064c28ab6d572604e9710e9f8492bffb71582356154cadf14a7209f8fb15293e460b23fb4f26e9d456280fa57a43" },
                { "eu", "67cdf3de42102a58de42d18380ef533ca0de72ea6f2541aee6da3e68082f8a0bb1feb9435a4bc7b7086c1baef16a4d3384ca33b0bf554906141eb241db504e1a" },
                { "fa", "b3b361bf9d939188b5bae7719771e2a5154838861e455ccc77d61248bf06e9a445720922984d8c095d4f5105104558d4cba3b772fb286db401e9146489fc4e8c" },
                { "ff", "898b749ba4828a4c56def6209ec7bbeaf2adab1ae28b4ec935d2c7c44cc7c1eddd615fe7b521667f18006a8b26820b670a1837b77a48be01aefe5c44b6d5b055" },
                { "fi", "03b02f53f4808505d8e655f0a8ded86775baf1395216d0a3d25702c4b04c785c3f5baebe71f29748a923df462bc21a4f83249691edab28b3c757d64528d7dd99" },
                { "fr", "179875c7b7240ebed06624ebd499086f77a6fe18d6e1b23a464f6cd247075d8055171bbe018f9721e068a631c44d2e801570fc10e784800e9734b46aaba80440" },
                { "fy-NL", "d2cdbdf0aaf0dfe0bd08b5373ebdcda4e87526c4c148b49a8f1cbe660c49440337b20cb88ed4f1cfcb474420554f49129782ba162ea205aedf210ac9fd1e9849" },
                { "ga-IE", "a4b83962237dcf40653998aeec462e4556a2d4265ed0bc633b6fe5b5834a7971e5a2ad84b24cda44ec98f2bb84dc7be09d89a5e4e70048f478ad37fca5085c47" },
                { "gd", "bd0dfa6d442d8b2ce06798430fce094e81eeb0085a50cf35d97fcec8d70712fed2e3367ea0f5523d825a587ac2f362af08ea17b8f603040626c81ff1da2c9bf3" },
                { "gl", "b9276f7a9dff11fcf907468f264867852446624e452b300c4cab4a9572b262eb9b40b47f3c7aa570fafaac13b958cf42ca0790f3be19d8272d88e4df1f2fe8f8" },
                { "gn", "d30a1cbb0d58eab83240c352c2f1a91cfd1da9784e7e490be8ce5c6154875e58b53334c9af5ec1e72b31fb1c64eb19712f5d1df3f78383667b4ff71503914aeb" },
                { "gu-IN", "c73ef1bb6ee4f8e43c6a83bea670c15dc0ef823fdcb6e9007510059fdf19e7e86ba0c2137835fed368b8cc2b1ec7c12e9d377808ac8fb67d7ad0fe0e795441fd" },
                { "he", "39eed94f5b43c40222807dc5d50e4304a4992403436198366695c7694fb8666be6717d7c138f22dc894d0b586d8fc3c0b9bf7fcee77172d0267a2ebbb3da54d2" },
                { "hi-IN", "77a7f457446d6fded89286ebd27de4b333e7564e3d9fa8cc9051a13f8f8e65756614d46f439d12e19ffac5c5bf22ad3d6d6dc4442094d257aca848f5cc6ffa48" },
                { "hr", "31ad1c3bf1ee115e12c4a9836be647eb1d650a59f2687ef6cb7b1b8d1da15dc9478c4c268a906172e2d3b50c9c671d8bf02b5765edcdd93250e86279f4a5c3de" },
                { "hsb", "3a4980bdf579ad2dc36a5dffa70320ec5df77f6287c75b3d7c1960dc0f587719617c1ff7d6994ea025ecb825b3ff527a189576bc882590202fa1f39b3813cce7" },
                { "hu", "f8c54e088c444082c4ff94c68abe44b81c09b4af5ad959a8e36d86f37c7cd338f1231c2897bf00edb4f799a1975269c1fbcc3f50e23701f1e478f62128217c54" },
                { "hy-AM", "b00c601311eae61cb661df5129c7665b272bf08bd382281a58779f9103170d5f508aa760d2a69cb38f290ee2bbea1e54acd2b064883169b5d27db148e5044ddd" },
                { "ia", "d1d8f45d6cdd7a461ed2f9880f685cb0c7ab97fe2d9c3b0027815be640448cbe590aa958ad778759f7b644d1107d10768b70370d1495de9e4546eb18b0a12a07" },
                { "id", "e6eb265e5b119212e5e7c44d59dffae2c5abcfb9664e2e0ff44996b60de02e577ea95beacb5b478500305bb6e1e849de4b2df409260fe9fa9c2bc2c73c570a8a" },
                { "is", "a8982c4109be45a255e3d46f1d215f8d32099a579e66b8be3e303de78f1ae4e8dda11c57a22cac7073a90512f403e4216c9e67d946ff0ca6fdea8e76bb3c9222" },
                { "it", "bcf72fd4190cddb409015021fb0a3d4ed2bd0b5352037c095836c0815a988d5e22c6ddd9223e7309b959f86204fa4610a819d8bb86bfa3b09390f64deae243a7" },
                { "ja", "aa7822fa054dd9677a63244fd05706694e5facf74ee1e40bbd635a5897dfecd68e794f486ccc9b9afd6d28cce87c73931227067f347b024a833914276252eba9" },
                { "ka", "efd65ef7a85b605c89b6a37e62d7b467ccf23537e3744ef8d691e44b07e6079a439502769d67fe7d1aa48288483734dddb42e78abcf6631f8468669c79af72df" },
                { "kab", "8a87d32411ee88e8203c711190afede2582c4d567eb9d03edcfc2b82924826968ad99d0ae718bd0a374b7a88d97f50831eb19d17e29411113d09e89aad4dbc8d" },
                { "kk", "efa126021b165d6d965e0fc2e050386637d9c41f37ede9bb73993ea68b52a586733c9a01b039794f1c4caec212478da6136200f152369138f710a5ef797fd559" },
                { "km", "86ff3bf7ad7e1e7eefa231c3917324b968d4eba52a379ec4552f7461480e65d1433c71565600937de82d947b68ee6e5129eb944ae71d098e99b12a7d708a0ec3" },
                { "kn", "b2c48f29822707e310f604fa219d090e0367ec15cf764b971a3e9f1db8a2b45c452cdec2da1f7c6377b418536ae093fc2cada8233a09c81cacad30e2fb7f6102" },
                { "ko", "9ead7ef857bda295cf13cd07b61e92940b368008a7c04a296b70159c2d2e44ff84ba5fbe1a7e6512ae1cdc95c17485a366ab1c9d66c879a3ac759b3223630789" },
                { "lij", "80a680f2480345f7fd6e370ed10d2cfe7aeaf73c7bd420ce0b1be0d680b7ebe915e17e9a86897a8e26e68b92828ba79392ea629321153d331a0fb13022109f85" },
                { "lt", "935af43989052211334d7daf959564408cd06c62813f33f717967c229a63366c5c2d37a34b2b96f84328c7b805c1a7e90daf5224d88afa0cc173d2d6188a2d2d" },
                { "lv", "73be565e2249714ff87a44474a2c9cb730090cfa7c33ac020e74fb47794d14bfe8b515b5c366376382c684746be8bf4a17885e8759c736881bf4c1e5bd6f25fc" },
                { "mk", "db56756db265d635ce3fec2161a1136ed28d1a4c793ee329cd1ae898a9b006b00c8cccdef1f31b143ae71d03cb0495fdc9c2b295e69450f94beeba3b985667c8" },
                { "mr", "03043428660edb3e6673b87db7fd91ca9ee84c67701de5faac13550bd8477d6d578234a0f8ee847c262de140813360620f72eb30af5feb7795334cac024010b2" },
                { "ms", "1505f6e2ea86f3aac4c4fce325a9b6ffcbbc8b0b4420ad6221c3a0d9e3d46772c0ff3330ce8a98c66616cf7604d244c10cb34e48a1b91dde2c12e4025478a1a7" },
                { "my", "50c44bb6374302e197ca8c376871509efb431b18298009cf38ef718aadb4a028e0761973b7a49c9b9a2d9247b750df035d2899b32848a67a0e5323349953da28" },
                { "nb-NO", "6b5ced7614eea628138168806addee3c7cbb069e0588776f63ceaa3bcfea070e7efda8679e5bd7d7919cf5d890df556f82efe43faf52a5e74bb516204dfa7e22" },
                { "ne-NP", "7d52db083fd9b363c6d6bca0e6840efad14242aef37dfc3ea9abe55b46c25e8ac27c70f72ecd55a2d83a954fafbec94272ce1e87c6c2b06db2d26aa9e6b835b4" },
                { "nl", "75bfbc6853cde1a3d0a523fc91a15512f556846f716fea8c333517c0011dc622be5197d1e5338e7331e880e7b7192bcb03f2587a8d2ce894c5b7436ec9610fa1" },
                { "nn-NO", "ea0bc0f5388e9a0f13202af0cc52cb2c5fd0263f879dfcd29a3f86299de0db472d4dfbc0e6a7b3267dc0df8cdd078678ce9e0cdb0fcadb6d474bc3eb6ecc373a" },
                { "oc", "945ae8c16b5727b13f1c18186cfe962c9fba7e3dd64270a92b5554850fcecf74918522f7cbddca03b3b802fc127f4a7e541987afddc78848110fd8be10eb5107" },
                { "pa-IN", "5c75245a7e226aa633cea1d4a4218f49efd452910d3ed99cfab28d4ff480d87cfe95e84d5dfb4d61150778d672e7c5048a1b885c1458df5a6756b3f8df452fc7" },
                { "pl", "d49c12ae80f3302a205c35eb262fdb0f96709f49871c82f9b8b748faca1af02d17f959ed74fe4d99ecad779a50f9be721a09d3be2687004670cbd75d41271c40" },
                { "pt-BR", "c08a9738c60c9cd790f363ef26aa092de2bcb2dae984a95a657f837a0524d745bc9714fd5c8c1590884c3c36e7b58a9703cda373ffc7d2c53e8c79e1b360dbdd" },
                { "pt-PT", "393aafb2cdf44db056e4ab8f5b7149f957711ef31116e24394ec06c8c6bdcf376617cc99fbd48b47fe440b4eb55a1ba485f101db9dcd5e7516f39de487c2bf88" },
                { "rm", "050e3bc02686556a6a7ed10fc9933a9438a2fa688289496fa6c6f216c49953f31ac27518633c186f3db233cbad016f65bda8f25f2c9094e029178730e474a29c" },
                { "ro", "526260460888c2cd176a1c2b538e021921249a43d7a880a3a0d9ea185238bffe3df6ac831df328c200c81b7958685071996b45eddae650dc6594948845e4069d" },
                { "ru", "cfa104f437231e22af38d255b89da8091963e50511f34611afe9b382ff1e7a6c2b57864d32c5afd8e31f559a3a43ead0ac295b353519363d65c8aa1ed0f74677" },
                { "sco", "8bd97e23b9c4bb710a5a8f8afa066f7cac3f948c0ebd2bf12aa9e8eeea8b39b9c54bcb3f9de2c6309e66f2ef3958605df3960db11c46ad58111d6d90c2338aed" },
                { "si", "748522eb6450ce120c08612b4a87d764bf017820767da4c1ee841697f3c5d682be30e9462501031e145326f7c329f3574ee36007c6ab13058442ca772213f2cb" },
                { "sk", "ee6eb01728b45ad0160c0e80693faeee9e5ecea3b154e4901bf64fadb319ad1e7dd553658b7ba3cb84426ad3a5f258a1ac2959eecde9cc2d6fbd606d38ef3baa" },
                { "sl", "3b41d27a6ca8cbfc931bfbf537cd04fd98387f0ae02004cdc559bb61f7f9e57a583664f84811c6f76c551b10c8e3cd4f2500bb316832bde87bb84b537eed28de" },
                { "son", "3aa58fc802eefc97785094d90dbd73db92c80cc4ffe4237f080dedd112e37282d2be27262c59304ddf1f9acaeabd4bb2b813c91fcfb991fb6af1f1c9b3597356" },
                { "sq", "e6f0fc95676062fae57f7e3b7f08bb015692a9d144f97601ee7078430c9a61d78874b5a940f3ce4b7267ccea70ae00e31d29cfd9e36ef58c972fb8b60bd85d93" },
                { "sr", "b94073f66ca305e9ebf0f5a6b60530f488b8a48745299c059205b5e637d5818fd944772f8d3ed1c605ca0d1c9beefb7982beb4f914197842851a271f8fe8e056" },
                { "sv-SE", "e1970518d3b8a8b2163d8da6d972e9ee872277bcbd7df6c239fb379d2bfe3fae88eb1288266cc302417b4eb3dbf0c31db958588d4c589fece53ace50ec12bca7" },
                { "szl", "9906c6a50c1dee4e9153cda0f79b48db555304ba47f950a8536e552e566bd77f74703e295c0cdea31e425693e906e681f438e33124cc8ae7d17d8d9f9435c85b" },
                { "ta", "b5ad3ab049cc446f2b17430c566ab88d562fd1774462092ee70129866dcffe47b5956f99e33be13017248c5173ba87e219e65c132a00c78c4d214ae4cf5b209a" },
                { "te", "7c346696c2aef02d6522ff9797bf488bf0bd1f39e2670315fc3ff40a7053c434d5da61cde43f28fe87b0f743f09294b13d03e0b0eb441399b80aadab6dcb7ba3" },
                { "th", "df6bb96627003beffc81d0598d5b655aa9d8fe1b3b90ea7fd0faffcb42dc16b3f1d4737f086a1d27176a578df338f38b0542d9e2ddc1d43e242cc2ce57aa0dfb" },
                { "tl", "f95692d86fde0fcc11b216b25bcdec6d07b74f85d507c03deb2d1502ce58abe63068c230d4d91867dfe88451d53d01b3f765d59705b671f93a60cfd50734f1ef" },
                { "tr", "04692fa91414850f673c80571b76b4c8c971328088352838d50793929e9a1012d8993ad6905547893aedea80a21c7143ac4fdd95cc8c9602f21e6b84edae4a68" },
                { "trs", "0db766768875e533ed07c51c9a7c9c437eb06224cc646dfdd2ea3e0de64fd3f4c8ce85cb6dc161d324c570bc5e3c69de92e4061102dc634ec29a5e4a8da4e0f4" },
                { "uk", "115478f716b7652e00ff6781c28e99c541445feda269121d088fffd50e15505c37d978b424fd3a7919f2acd195016ee8b1a42026a1d693dc0c348f4c4adf7133" },
                { "ur", "0b1e57116a4b8473efa1a49b83685281714a2ef624bf2ec207d4ff72349e92edc8a4a00203ef766a57616553e96565c87249aefda168de3fae41d93dd687479c" },
                { "uz", "86855c507d1d1ab42ef71fc6f741d9bfb4004066cdc7c9669b18822cce5a97bd7849de71998c7cb42da3aaf46eff3c6315da1834abfeba0c2089c0ca1a89848b" },
                { "vi", "f5074940a5e0d0c4e547e3c4c4890ae6293cb18e1894de05e5dad5737a7c5699ec2f6ed3dd1bbf4878f6e22b3ef58f5ce3880f66f59facc621407ce2ab1d8039" },
                { "xh", "2732a92a22484613d401fcb000adf574041275e16334aac43bd078c1ba834637b26f25f48752274e6b0c624a7138ac0a2c6e4da92ed9fe0f6031f5b4238773c0" },
                { "zh-CN", "ec0899c4e131fe327084811bd20da3d50785314c827003c4f35e4eaafadafbcdc1a7ac8d7ad54f547a9a6d01a179bc23cc59969806f84e53e7eb6796e5638ba2" },
                { "zh-TW", "d40aa00145ded6b205f319ff6660f15bd7336a96f4791188adc86cea20dbf0c1053d4c58905606a50dad3fee709a48ac6ef720d219095cf4aa942b1abb96f12a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/97.0b7/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "9fb48aac355438e8f9abb4abccfba8328e528f436cb410e9418a2f41967b9f55d133d115e8643b8d3015a4ed2e48ac16ea65027e484da6a53efc82df847aaf9b" },
                { "af", "ffed05638e3de7db4f14c4cdc9183072503b307ab92357e6c1335071582530b115cb375b75b16b576692879577c0c1b9f37c09bb8666175210e8e60787e02ae1" },
                { "an", "428f941b81141042494308eb710944861283424bcb50643f3c9ccb0f92e7d6acb67c0cd10769e99efc0ca5cd2853c00c696ed0c982fb6264e66c4121f6b9e193" },
                { "ar", "fde23db0e02ebea2ebc81261c3c47aea747dce15fbf05e4b586abd46f94cb2a6276d6dd3c4b7784f84cbd5bfcf13281eb690762f062a2ae0bf913b180938460a" },
                { "ast", "04780a8595167f68d58e3a8d5b7debb68dce5169a4f1806c54334fc862b52f3f116e752a945b35cd21a4e48ea9286d495541b284ab8404092d86cfa65f153f13" },
                { "az", "e92c58b2b05365b1b619e2bc5cfbc8dd3f1e738014dbee4f38c516fdd38494fc4cbf6c9b30751766a39dd088e87bbe1e26261ad87eba9f811c55138afeb0fd5b" },
                { "be", "a9e5e4f13cb4eeb6fa41e008ec4ddea12bcbba4561a2ef9b1636a636f13f0eeba6abe6c637d7124895eced059d575a95130f819e815e847602dbaee04f2d8498" },
                { "bg", "35a39a93f0e47ff5763738a1816eb25f52df55e8e22e25514549f420a2c69fc9ee54ad0a379755c4f408477228ac3898cb9d43ac55fc7d1890499ec612c967d0" },
                { "bn", "0284d319e736e9827dda9ba9c4e6ffc7660719b9999e3e98b5eeb152d455bae43c2f1a185b71d585501d7e670161b4f8d4b10321feb595268ad01e9f24a40b88" },
                { "br", "51a5dea0aaf8436555b24e0de6450381509bb9fa88b40b1186ef715b8da48b9bd4251f7249381e7e2951d3abd7f3b7bf914bbb7a31d0cc693efb0015f6a16cb8" },
                { "bs", "b48dc71d0e03fae53d1499de6a3c702b616921afd061edc0f02c8273452f223e4e00f55ab9e400e98329c75ce5c379ddb2b7d50ba33662a1266fadbba1ff39c1" },
                { "ca", "c7c1ee825102a069c3843bd1708d0f5719854021bf9c36fdd6a5ab5c1f6258fc8d0cf4763655ce304b0038d2c2989a9f3fc0a05a163d22e13107a5b73087baad" },
                { "cak", "032838ec57552a37ebab385e9979fca79abe1befd3e63f89673e3d9142428b8b54640dde092024c6c07699a87f0fc7de0107edd7542886ee8f26cf6b75407cdc" },
                { "cs", "ea16966b15051bc5fd33fa7ce90d9d70e4e8bd88a03baa4575813a034bb2281a1b8b61216df4e00357dfe8a15144d1724eaf8a83501664dbd79de6735608de83" },
                { "cy", "f146c279ff80a90cbb6e722d64f783f8984234f651969f42e8f428802dce6e4d95a9ab41ab7c0f73d7ba26ae896743ae2a6ee1058f6f5accf43187723f6145bb" },
                { "da", "729fbf66710b58ac72e0ad0461c0e00a0354d235060a71534a5bd8a12f79a95cc75e2cf3849af0ce4527e22c86befac52e7afc58bb6b7925d5ed0541deac151b" },
                { "de", "2561d8c970789731d6421c40e3c37fff2955aa8818d1f412c8bc71fc59fcae069ceec1a196c6829c813c1249b95aadbebe42c3bf4ef0b3789c0487467f55c8c8" },
                { "dsb", "4bad052331206462a99320a9c7378f0b4dfa974400488e88506755fab35f245b0c115d5ccbd510974138590410feee0654d7634c566d8d77099a79f6fa0cb036" },
                { "el", "b3d28a39544ba9dd918841c12d0a8dba59657bbb440e32403928da9958cbaa73e20ec043a32876aa048570bdfbb3376b1d42a85e59063fa6394969abdd5a81ee" },
                { "en-CA", "a94cf8de4c05cdd6fe2a05533fba0c3f6568961c2c34f46b7eef0d42f4a0e33c4aeda0aea8135091f5b1796ee731db8d48429b5416334bb319df08910683316e" },
                { "en-GB", "a548afc96e03b743be335e70554f1c1a0549476e874567f28b4a2299c480d0014ed31596c24e580fa784c4cb6f97b9771f09bb4cf1aacea748e8a5136a46d041" },
                { "en-US", "b521e7fd73bfe68487e70536293e58719d26d9bf638bd5d69dcd542513a815a5d7fecfb8c325de09604360b75b59f7cdf9939f51004dda012d1e3c87438653f4" },
                { "eo", "1e3704e3a61802879ecadb96d93ec2042686e9640bf6f0ab187fc23b0660d692a632a40acf7b753e7b6b3c33f907aac8451adfedac07cd11580f1db1e9ecd29f" },
                { "es-AR", "7fa10174aeb1b9e3deb5cae1c1f2c72746eded40e5525778803a0b4c7b7cd3c54d7836f81a2e27047714f9765b208a3b5852c3dccd7b58a327f803f4955eef30" },
                { "es-CL", "b0e2ea4a47dc5a677394dad9e0b99d820b5fc74057aa78cd0bc6ceb113c78d55b96aa2e34b3405e1e2af83d856f7322eafacfd37ad5c973a94de44812cb685ef" },
                { "es-ES", "3b3fe8c2e12cc214b461e6692118a998719f1a4c43efedd0aedb22b3351956b998938f036a1642a2d5e1044598060f135b39d1a8fb90788b52a2dfbc5af2928f" },
                { "es-MX", "3a172e2f7dc36a43e44d16d63e881d30060ac49848fa72021cab8a4e11c0ff96f0486db2a54f0ea1c594f1f31c348a4a75a2fb3530aaad232e16bbdff1520dc6" },
                { "et", "ecebd02ff347e4057ab2002e4826bc81a939ccd2fc4aba61e6e063da2e0a0886c96f0c9f8c91074cbd25cd37b1f90f8796ed8f0ed6aa616c0d8d57fa8ec6ff1a" },
                { "eu", "80edbf8e97bb4ce327d0c65472782f6924d8ac1cd6d2ad013ac5daa4f17b13e9c77afc70c062016390c4f9ee6ea4de9e347e870ac6dcca54f8e7f6fb7f74f36b" },
                { "fa", "749befed2083d3883020ebbb59f78bcd333184968f3f2f1b1ee978ebaf006903a1204d14f5f46c9e069b9b54fca40a0c7416c33f0b33c8706ce8ee94eaf0d9c5" },
                { "ff", "546a95926616127d98cf408becaf725a0c418e80882d89b96f2c92b0956a09d1ab2ab9a5b20fc5b68e2e642d50623b1b6a79bf7fc40f44d1a2c9a51654b70139" },
                { "fi", "bc2ce53ba8d7fb925bb316d0f80f77e555340b4c530ffba9571c4a34c86c62be85c3ea077d3dda88232b3508398d15b9985aaf48cb20e83a6b0c8a38cb9ee100" },
                { "fr", "34d9536fd1ad92d2b8ee3ae8450f2e3346b768a415140eccc96e885b3a2d6bb62099dc658f8674b32c79fdbfa0aca10a653024c340893a83aeea6a4b15a1f682" },
                { "fy-NL", "4ae00738ddf9adccdab01645a02406f29c125292eff16fa1f6cb99e9e87888c00e10e34c5687f9fe5d3ce8d51062af581710b98f4cd71c1ae82c7d5bf2511883" },
                { "ga-IE", "de955ae52d4765563f554834a219c5f0374328ae40a1dea07ac65bed8603911ed08ecc2f9efbc713dc84fec7e5bd3bfc483300a7ee9881a6ccd30caeecea6c51" },
                { "gd", "0fe0a425429e5fb760f99f6f799c2ad0a209412f6fd2fc5c084c6307402c1c1bd116f1a30825c83f4e873848b25130ee8409123e638895a5c5219b2a01d03524" },
                { "gl", "c9752cab0d8fa1090b9e00ebb254c3810100eea4e81739fdfe669f46eb07d9f1250bc140cc6c8473242661287ba7e0e3ff464db2d6b9dee9a066631b80e75c7d" },
                { "gn", "ea1b494b485c4e2fd3d266140c5f6fa82b3e2f5a51a604247a38d39ffeb21dc80baffc181a032963ade4a4c2616a0b201362463248196c33752eeafa9f28e666" },
                { "gu-IN", "21459b93c88bcebae7b7b5b80bedf76d757158b5b03885e8f7239d86cb262cb53da9f82a0d8779a2c3f84fecd6ab674cae7ec226cc6dcdc380f611272cf4bc52" },
                { "he", "dcd64a531cac7217d2276d27825bfefc9985e1fe515088876a9b6dfc91c6a0d7eeba7d7cbd90d59e6010f59b572f1268128a4b014a5ed9861db7420942ab06a0" },
                { "hi-IN", "a8a7b079fb7444975ff929a5bf26bc81bff8e16e0eafd559cf0d1f87d1c8c6621dc4673a1e188d1ebf32c419cfce5fe70eaf5af0b93e39dabf1111bf27af6da4" },
                { "hr", "9cb5fcc7cccdd61fd655d613cceefe7bb4670e3306bc8226ea9bf83f74db45f469e9363697133c65dfdd125d3404e680d908d4769d3b00739194caca497f01cb" },
                { "hsb", "344d986e4e485a55e49bb1bb4346d81fd6c1d11c89badefa6c0022d33113b7d7057766265186b428eed629e697a8986d0136b7b688b2aaef92c339c99c027022" },
                { "hu", "af4e5607d478a314265ddea29502447b8ceeaa068865c7c3a6611b533804c9ca12823672dae6b7907e3b3131250d5ed2ad8b987f0954810a9ed0f95d2f842166" },
                { "hy-AM", "b6edc60779ad98ca6e713837157443e2220c428c7096a650189b924045783105ecc3f88d585601bb62583238e7010c919957f4b5b8d60bde19e33519552b2ea1" },
                { "ia", "a9a7c3ed85bb6fb04279181fbc8ade3897bb38e2bd61b87b37a7b98ba456fb8137ba66a377ccf11887acdf6925977abadcfd2ae916a511b080e2cf9ce5eda9b5" },
                { "id", "d60a244a2935646aebcd6a8113a708b2ef23a194401800fb4dad7966d0d51aa186ab429c9a71879310714d6068fc60b3be3bbe8e092e071ab165f5851af25710" },
                { "is", "89c270cee050a29fad4eb6e980afda494d8c8a7cc94e653bbfa96a531605c8a842af475044e10593aacd34cd69f8dbd1448cbeebd5699be77229b161d377ad6a" },
                { "it", "d69ebbb08bc68fae78a0e0cec8e1b20f4bddecc72130429bb77ef1ef2cef6e70049ed9dce3d01524a2c00a9b6fb6fb922f4bb1c929a56b29d087a7a4ddf2ea54" },
                { "ja", "0ead38ab281b2fcce7652eed7ec9b1990b06aeb683533c796ef8465028c1f929cc77825d23247bf90b4907c1c99a77b85edfffeb82167f0a465d0ff51330161c" },
                { "ka", "3d11377088916fdf538e97600efceac73a4251c7f727db179a7a884ce9e44cf610d518bf3a810cc60e272a9eed6deed077ae1d1dd3581da89e9be1fe938b4b9f" },
                { "kab", "6a46c2230c5bdeb80867d3aaa022e1df23b1fdb72434e8ae8f0c9f1d11668d8e6ae8f602c4344b2ea0131ff01ecf314d2eb86136cf72db3b417af51bca2bb6de" },
                { "kk", "858b8e1fd7efb4cfe85d1debc975bc6c54051949d6d50b448a3d0a56b3361992d09ef113dd8ceb5e2bb436e3cd5cecb3fea5db33c6418c0df94805800fec2916" },
                { "km", "086754bc3378e880053f3f2cc3fef75ef02be5f77da0abcaed945036883fa7a5da4f997bf29fca8734710a05bd98663ead22e94a255e04d6edf8bb74b8a35c30" },
                { "kn", "234491ea9dad199e739a36d50e645383e5f6506efd92bd260c7e9c1a78e5034164798836cb80b534f6d03e9c8ccc574bbb5d026f07b90abd66a4ee2c5e860eec" },
                { "ko", "847cc7604bcd488916205702ccbf82106a91f10073445994dfe4b18d005b7b6fa295a562df90e2145a4fb16c4cbf8c3a55f5c9551d56be6bf35dc5d870768196" },
                { "lij", "deb2abdd9c35cab9256282fa54d9e4cd56f049dda0ab1bbe78239c09bb02705e1ff274fc13023e755b53eba89cfeb7cec09a9a61f686f31c3df803dd831d4552" },
                { "lt", "2f5363d5bc2b9aef1c06556516ca47989b2b3b54fa9624848431a3d64838dbeda9c181eab5fbb8af6021172c66e8afba67e25d9beb9bb4090a84b91f4d46dbce" },
                { "lv", "df0753b8e20e7d11ed2d2bf5525c8d79ec5c1cd234026fbb76fe9c6852a12781e16022bf2a1f3d2a36c0acd7207301ae73be63b0954dae654db070ca29867ad7" },
                { "mk", "97913e1d07b2ef799cafac54454ee553644752d2d03fdf889fa233b9d4887652f8f7f41b4cac1568920191f49a0353d72bf04a24cebd9e219d8f190e27ae7e96" },
                { "mr", "0dd3c902647564882160c7e569438d9b71ea1382d348c9213f2b9f637a851982be21be51df5b18993d92fe783136da2c9ff06655d237ffcdf4351e8b18d8f2e4" },
                { "ms", "9b08a4739e2d65fde2f40ddfa3d5bd6d268297b8272fcd15dad8a55652cc29250559821423cf238435559e81883fc51badbd96c50979edc15ea3bca55d24ba45" },
                { "my", "c40099f3cb3019669e9901dbd52db8b9eba67f2e39ce7ccf1b5950a380006e0d26c545e5debd6310773611220ececcd1b60d79fac272862383494e266555b985" },
                { "nb-NO", "95907c0061eef61c5cc9f7c548ccc8ae73498edefa12fb6c1f58d41f9fe2a9f39dc0a4b462fa76bd1825da85ec7fb4f77b5ec3da4b60792f85a92ee3d430cbbe" },
                { "ne-NP", "3b02629e4c3212901841ac5891f2f8a3bf90d30a07570fde61ae29a795e32eb5fdf31920015ae7aed03b4da3313d42b3695dcaf99dd981e2761cb1eb71d87b1c" },
                { "nl", "9870354fb05ef35532c7a180e1ce94f60bdc128ddf9bf6ea6991bc2d65e8fffd45ced5204e45848adefb32f934616297114c361081d0c43671c3bbd01801fa1d" },
                { "nn-NO", "78d28684227924106af58b15b77a09faa073df40e84261a18df569182a046d537f5d115f6b28577e1ed9fce825389b085b5f3d54733bc61b78c70870a8f05d09" },
                { "oc", "d3cedec312a26f15755224876513d052deb4190eca6efe776af0c4978eb62c1dde6d42dd899b9251d070560bd6665dcd992dacd4d726ab72d40c3a6324b02f19" },
                { "pa-IN", "1d28dcaf944ff0b8dbce690394b53036b8d88ccdc736e6a310d2c337d7055fd6683b2bb33d5102f37c278d05d850b77d526d5c48095a827bfd4eb3efc6896ac2" },
                { "pl", "a5daed2c6b42bb8ecec592c64d4f5851e465ed71938d924a77f307b38909e30a3f09acee6fd522b3353f622d981bbf2a5ec2f1167e3f6ea4fa225875d602292f" },
                { "pt-BR", "d94c56586cc1262f8867cfefce2ceff0424bacfc480ac4af21b23fdef971cbb752504afcced2c720ecd87d88c10325ec30fca08a68c72f9329126df00b17c5ee" },
                { "pt-PT", "8f7c8b3ee0da0d771e67409ed3137830df84ec4f4f14d509b1615a191e5cbb4b77445b41a727dc8bff81b776c50b966cda987326afd62f03d5e0cc619d56f1a7" },
                { "rm", "5bcbf9b4d0df4af9403e774b71a71588c15c95d219c317a9aab61ed13e6c346a85f8f3b4f50d0528d19505c40ad989bab05f14cbea7963c1584ad5470ceba78a" },
                { "ro", "f0abb576c5a8bb607c9fc8db89c98fe2f48c75571ef367c5675ec56c5f8b464c8265d5ef2bacd47d9002676dff2dd4983ca12a134beaff90b4566b3b20f41a98" },
                { "ru", "2dcef5277d55dfa3482a66c12d9c9e818dbed87e698da2032db020ac07ebb9690490f6b6f5ca1cc2d2a95323d22d02f9805d77761d5d22bb6baac7deb1869577" },
                { "sco", "cb0f53eb678cf12765777fe8cdf5d5f3db8cf22374c54f509d7ff1dddc3cd33d54b876f7d59906fe399c1170f518470a17533a91fff186d1fa4b385220f7506f" },
                { "si", "b52ac25749c3780681a6179e18154c50c1dc3912e476ef8eb31dbcaad6b0e028b35a377594868515e86e633f5cf3d50eead03968728a1de13a19d5be6c8230bb" },
                { "sk", "1b4b9fef999c8f8c2697b0c6837605fe57ce5e4cc8f9e8904c9f2e1ca7fe3dbd5632f8fc3673611b5ec3a254d318c5d9838a59a04140d7810106fe916a140fdd" },
                { "sl", "71f4965a36ed279e4d083c70cd5642b015faec9f0ca6b7fe9c7aac88796f21cb62ed16d556da94776560c2abdd00b1ec59ae2b4f02b58869676dc9c739a7611f" },
                { "son", "58da16773a53cb762ae6153a2803927e0a689092519cc1cee483bc4330209bf347768aa09f44f71efc23a5378e7f25051e8871a58204767acffdd22d746a5cf1" },
                { "sq", "561e52b43c14708fbd6f753947a42267314f6a8a2bff0001cbdca1d268ef5e91d12c858172b060b8f4bf18731af8a965513055ca58d8d722a96817a26956dad2" },
                { "sr", "e7eea758e07161c6635188bcf21bb65f4551d80d110f25b431960c86383b8367aabe75d62e26d941cb7056639ac8738c74825e63ba359d28657a7977cb89c8dd" },
                { "sv-SE", "a49d4b659f714716125a06418ea386a8862dd1f991005ce9ae1bcf98ee6ababb4d1985ec0d4755f5e368b620bf0f73c1ccbf38f18eec67e99d14c536439a6dbb" },
                { "szl", "520bec3703bf5e2b24593d3a2c8d260a8ddd8ed6377316a5b62b7141ea3472cea6c65e319e3872964644db2a9087dc1bea4a6ccba17c8be633d8469f251ecdb0" },
                { "ta", "a0c2772e1e864d6e93f41234072f544dfcbd8edb25508781a5f1459ec2f339f27dff2b7f02cc456ec8255e6f77c073b94e36f0de1db606d01b205596a17dcf0a" },
                { "te", "9432627ff33bc08ab3d475eac98927b8f47ef39d850c30ad14d2ed4cb3d17713f2154729d94d79d261bab3ea97bd8a62fe92849a311ce5eac5c56bed44b16cce" },
                { "th", "4301deeae1a5f563cd16cc027f0f50e51a5c2e65f453d8a42e8e44c2936a3f9a53fdf7e810ff7727d3841a98e3680d237d6ede0ef03f832cf1a82e06f7a7274d" },
                { "tl", "6b3a3d569a004ca8e87f41be628fa84037dc8fbab561f3b1f5b76559f2a7312b770f3a61ac87d426e1db75b30700aa188495be8749e5ef5ea24bf55a78f79d29" },
                { "tr", "7e96c0d2d2b319822f5a88a8269063c236a13457dfa08faeae436dda1ead2eb54d0911fe3438b01904f26ab925039c85db5adedaed7450d79d0d72184ef9974a" },
                { "trs", "ac45c94644c69602c8cf23eb52227bc5ebf0ddde3ac5e792e2f396f6ce0a064767a3bef7ccc1cf00d0802a197f1c9a1100eae05b005190c25522fb322a3fd75b" },
                { "uk", "cf5c56c968109de1fe49611abba8eb787088d96d6d852be4d072f5357f111133d5074927e6dbe62dc94ccdb079caee1750a13b003b370db16c3c63e2943f3bfe" },
                { "ur", "5f68489e5296f0706652304e85ebf656af010a797eec63ed681b656ed3bb9587b203e2e72721917e6817bd1f90c666a02d26cd76f04e167b6f86ba4721d15aba" },
                { "uz", "7800093ee153554ff64019645b8c38fad13c09878d175f30ff1489ef67078e3961b15829404e15c3fd791223bdaffa9538099fd5cf4071df404af8c82073cc7d" },
                { "vi", "caab2266ba93c06fe92aac9190d3c1e0a7b62905c23875ec6a8797ae50224106a1b5144eac7ac5813669b7d6f5e470943615cd04f3c896426af65034042c4207" },
                { "xh", "4ecdfb408f2f66f4ce106b78c83fcaac108e0012f90196f7de509af044ad13105a009fe207bbe89dc9778409631ee8b9b63d43a7e4dc2de66e5e09ca3f713e88" },
                { "zh-CN", "aa6077df7c4cc3e30b8553e0db56e75e34a30b9ca4377188990d9195c3a5d2448c480cccdfccc280dcc5410bf6ba083e5a3175b869d87e1310481c50059c316d" },
                { "zh-TW", "f4c67ae954f3e8e6284d2d9b781691cef90db9eda471a3b48a4444ac62c5f9c724b5086c91f3959756ec8bdbf09f30212c89128fa0e82c8a4ea4573ec02337ea" }
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
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
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
                    client.Dispose();
                } // using
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
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
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
