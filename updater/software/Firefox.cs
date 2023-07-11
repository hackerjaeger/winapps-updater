﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Net.Http;
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
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


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
            // https://ftp.mozilla.org/pub/firefox/releases/115.0.2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "820040ef224b8d63ca5b2d24c0c9c587124338996b9f88459dd76b205c990aaa49023d6934a35bed97ba2546ac9e352c580f75f9c414bdee6cd79607e786137e" },
                { "af", "a286c7e2085a269e5df92f0444aad782d1269731c3add299345bd8f434bbe2c7dea326e2e4a941b0dfc5e5acc199268236a6355fcb01852260fd035508417283" },
                { "an", "478b3ae831133f843fce46fe8bfa2b855693c496a32764457b5f0c52471ab6ab51397243640f2d10ea4e15a2e0277f2855c3217077c8bc53ab8b8a53885a5df6" },
                { "ar", "3910a6d4894249b3a59d539412a61d7299736ba50f2e9c63553192a8a0a1faab541edf51e5b99706c687ddc09fd1943fc4cb7334f7d5f1f350170b1c3085bc6a" },
                { "ast", "7115e817e69ca6297d03cff44ab804136a12afc52f8e24d53b742e452eb6b269acbc88a9815fc097f0e4a631cad8819972555c75ff4afb9f4f15fdd867a1c739" },
                { "az", "c7a0e1b6d729d5432064ca2915c4a0575931db2cacc0cb3a6970fae92b3c5149f43043939c74fb2ac4e28ea04b6316235c896e79c0fb5598845033b2140003e4" },
                { "be", "2137cfcd564fca572d20d8682e254d64f7b687ac29926658ac0aa350d10b6efc2370a1155674956414dc92d7daeac48a01a1006a3e0d2c1740264dc5ba04e445" },
                { "bg", "b7519b39440fa580a2ed637bd7cd7ef7144c490fa423adf3777034c62752b34c199b710b56d170094ba3fef603cb07f118bcaba51327766ef4ea4966f399ad43" },
                { "bn", "26c454a5e74efadc01fded9ed0b2196f42338b07d59fd66fbd148722a535647a97ee0f17cd7e3eb53eed19b3bb73a5aec45ac4625cb5f369eda8c84e16819939" },
                { "br", "918c5acec22a63c21291253a94b50756a549a02d85f8699ac874ba6cfc20ef2c489e056b3bd03bbe9861a199645b80cd0626c630782cf70a3777626a8c5bf493" },
                { "bs", "01b879578ab8ef3a53f5e34885c40acc4eb659c8da4fe0e6405df42d0072d2098fe201210745701248c29e4523561ebca89bdd9880f213d224a6455face08b0b" },
                { "ca", "dfe776571e1aa23f57e0901321eeebbea2b7c314c2281949f4e720c33851aabf289548d2b9abd86aa8f7707c74372a23339c3a386e4dec81085e5945acdb89d5" },
                { "cak", "9a5f98cfc9dfeef68831f9adfbc207e7bf4164477b7c18f1e5629adff282fe2208294f53a6b7cbe9b5b422f5b6a31d16d5cbacae696c3c009002dc012fda5ac4" },
                { "cs", "027fd3a6bed7fc2e7cdee715e6a39357d1dcd467bd31500eb714dc013cf89501db17a9cf5c9be00586036af4a0ba69e1603aaf7cf938b2b077fe5faedbd1d2af" },
                { "cy", "c9ee4707c182de4499247ffd5eda09d44b71f4a87c315c023b7af344fadaf16f0eedf60396b6810ff8803af0c493168f7657e106a60ba97ba803250457a16c01" },
                { "da", "8ad25a6937c955c63b4f48edac976c3027245b22b217c44f2de622e3c8d021cc0140178f2b506b32f8af143280aef4f0bdd3d46f3c2fcdbe31a3da433cf7e5e9" },
                { "de", "05fb3de4322f92e4060226095367322e8a3b5badc7a042e020e6b73f14288f891e3c020109b6f2b93d6e40a97753d2fd1f49618a0b66f1cfb1924f7ce37beb12" },
                { "dsb", "4ea21f8c04e75f555c7c02ce381f9681719a385e995b8ab4192b652f21dcbfb08f279c0679485b52641098b7276450726b9fbcb0d380743488c900ddd3390236" },
                { "el", "bc620c704ac890931cbf2e65c0566d5484fd39ad9d1016c552fea5a8dbc577702d007a2c124a3e2b5288769f4dce4f66316dd609a9f7038968a4766732e715f8" },
                { "en-CA", "cf35a15e40d995263368f8d9525ff1379e44914a723237a10509aaf94ff34e18c88c11b0d86a337d175274b643ab5c5101ed20cef78cc931f5ad083d83c6bb99" },
                { "en-GB", "cbfa600a3e161a22cfd47d38c47e09282fb30bc5318d3fffbe935beb1b1a9be0d95f8c1d0288b529e04d4f094137e8882bfff7c5f0c65d8e9d94086635e25f56" },
                { "en-US", "085eb4f450b4357fd041db6f9461300ebd677c62adf148967ae53f423bf4745491db5c8fc5bc5ddb06440e9b1125506b3255793618355f65e80b0f64e3af3c00" },
                { "eo", "0242bc86494e08a76bb573b507d89014ff5da6d7c9f3b64d884e301311fb2cfb068c26150643a1d278b2bd85827a131940857dfb3ae71074a436e2795293ade4" },
                { "es-AR", "76d1e3ec8f260afdc8728309f989addba970b965f7f80d55c0273665fa445c9466a4619b7f5ddc7c7583b3f1b07c10c5a929b72a40dc1fe29b88f4c4455150a5" },
                { "es-CL", "f99ec965cf3047352e92d3cb4e6ad0f396c77536e2f123b57bf8250403401e0fbafc29d79cc8da38999c4b885e509bb177f4a0ef39ffd714a05a7e66faf3d1b8" },
                { "es-ES", "67da3d5eee1a261dfc9db28f590ce9564d37d0bd2345c416511a0d2ef3632b825484c3fe5c4d005f5dd13296ee605d10d61028b0f36b770d4bad9be7e9fb72e0" },
                { "es-MX", "783e67ea4725933ee09f4a195915ef672fddbc1894cecf5acee087df69bdf352aef00a31162d646c4d6c0796656da27852efdf2af196105726df7337c9ac75bd" },
                { "et", "0e4890e7e1c45236579230724138c92f6bc0ee40e9685a062eccb2239714dca523eaf14c78ac6d8e8d8365e34193f637fb7800c5292c1bdebd683ae9209b2728" },
                { "eu", "61adab4767471b87fde47826e4a18e60dfbce981ad115168aef0999a07da84c9405079ea3c90346cf6aadfe0b192b5a2960b6881ffd0886a850a83c2790b7ca4" },
                { "fa", "a5ea8e990fd577298468981a2fff7e5a30ea854b573460ee1a8d84c0c0bf9951472c88145f06843f76336f4cb4be399da2a57283af7e567500118dba46391c8f" },
                { "ff", "0d9e26bc2c611f9957b001296991bbdd0d1bd7a6434d0efceb6747f05da900ed2a24ae7bc5e26c66a86a5b53bc02be2a8be99de754d8814224f102365c29fee6" },
                { "fi", "c73cb606d4b249238097b3003088f9694217489c74815275650a48e1e2dc33c8023abdd0a192addb620ab7df43102c4c6ff346356e5a41fffa7326d2c05fdb42" },
                { "fr", "3b9cb7c591d2c328fffafeaf76a07460ef1bdc92b41e0ff6e70c7ab63934be41e7231271c9e6f14fb2baa4bd701c0b0cca093280514729a99d823bf540e054dc" },
                { "fur", "82df3425d97efda9fcb8a7d0bb09926b6de207edb35cc15fe6a9645c67e0b8884e14b7a3cef4516209f4bfddae6e910b11d6c258c1d8b2f0da7258889ecd6a97" },
                { "fy-NL", "77df2f861f02aba204d834e7673d9749c64f38d4db03a1aa5bec1718027c12157786ecdb6f06d48f259c2df46fd1b45ef5bd92cffd7302843d8416d501ff41d6" },
                { "ga-IE", "635df34b675f9246af84ee2a5eb0f196f641653d64944e8dfced75463634ee2bc2017b7a408a708af67ffac82819c90d51784d6701c8037b3b128172c0810c36" },
                { "gd", "cea576acd0ef46cf05434f52063c5feaa90d1c729dbf8fa15f4bfbf3ccabe6b2daa95d45342fe9b7630517e953515b5ff3ed523b979103fb7d3bd930d2cb38de" },
                { "gl", "874d780122bb6ecc55abdc46c5db4ddf1bd5665141d71d049ac1270074664e14a8beb1f357279b46c762c975f776ca017256ca378971fd6d9117f0600fbc4d59" },
                { "gn", "f2765a5b1c1c4c71e10a197a0bde1eaf28594d04cc49114427ea50e174ed1ba257c7c8361bfc929f1e7d35b2da998afad6120e37f7315fe7d8f4d185e4e849a8" },
                { "gu-IN", "7b1cad46c6a9b2a7c7f21f25ce253f3c104363f62220c6f4f2da314df31aa01002baafb356d521d5fa1754ebc92fd972b64a91a6d5bcdcd6463815c23dd3cd90" },
                { "he", "406870b872b4a74a95648a1e0ea244026ac4f585c41f4e4e8a8f08d1e11e30a79cc662cdaf6040de90df13a5677ac81faeb6d4a10ee75422e389ce78c05abdcc" },
                { "hi-IN", "da833465940d17ee517218837b974b08dbf45d7e0b8290ba13f77d83388196b592bca58079635bb95c0561726a38595bafc85efadd10268a30b8e3006fc4f344" },
                { "hr", "84a20886d73e698d51daa5776d026294eb8d28617dcd949f4879b87f7692ab627984efb53e8874c913851131d2f57ce3888996657995270235fcdeb3e1164197" },
                { "hsb", "426ec79acc30fd151836b68af7e1077b0d7bee9132ec8e7ffc4cfd94dd5482c462e5baac4185219141bac21a4b663ea33f679deb84cfbe80491dd519b13a444b" },
                { "hu", "4b1b65e49d2d715c42514067fc33426a5155399a90f8fe0361e0af28672b6b94bdf81bc7f6c508fd293bbf034a9cde18bcea654569027b0140160129efed6576" },
                { "hy-AM", "897d9dfe508e5b33aa7bc9ba295b68affbfb97bcd93b6dae9f1c05ea5f892ecdef431250f7bc4e580dc3f59ed7d16f3e37e9e915972a14c5e02d91aa9cc5079e" },
                { "ia", "d04e1fd20b46573f84b2b8ed38e90d745ffbbf9f085225da9f0b2d6e49c694b88b9ea6ad4e6bbfbe5f1e5d0b6ac60e45ee5a1d64e472c15ea1f074cdbac41769" },
                { "id", "d9cc70da97ececa584adc1463da819370ce745b9c8b5766384892f46e851687a83cc13ae71cce10dad0ea4684591d42bac74f8fd664f71c01057eeafe56a3f2f" },
                { "is", "ae10288ab73bb441c3bae7bb999e14c3c85815a9286adc09bb5e9f74a04856b221f66bc054fda2eaa4991f78148470367d7d6d2e223ef109fc07f53caebec444" },
                { "it", "197f9ab047e2a58dc84f287ec5727df34dafc3b7e13df6a026dbde56b57c27749398e607ea3e6e363b2df373d3df21d73aeeb49c1180b6456a03711eaa453ba7" },
                { "ja", "08922cc59b51c6004dfb4cfdcb5e20d32405ef69bc971f6d8654e62296f353ee08a7164bbc67cd2892f52130e18469edadf03c86fb0027f7abf4f63caa96ae42" },
                { "ka", "b2fb21e991c33c374e8a5d9dfc8d063655fcd3244a09930e6ab63cf41ba671748acbff377f3d5e95c85c36d661d2a0eb8b8676a1e9b1b57beb721eebd22ce969" },
                { "kab", "967aade8359fcae618e3c4c059785cf5325ae306e2ae3e501dd64ce336b7e5c7a5b80b8bb48b24fafa686f90a1fa08081362aada8e230395cbbc8192a79d52e0" },
                { "kk", "0cd944efa345abcc617eebbdfe188aa5a6d491c7bfa6c02633d75b57b8aec9df29ddd4dc967b3158ca6ef87d58c5cda633dd633b178d28d0d5632bb7cbdee4e6" },
                { "km", "6112caebf5005917016729ca5f1c2ed034bfc06f4c386def4560558b9b9a6abf4cbfa6ceda23433f2c3a538d58349eb1b341b634b3404a5d8c9dcdb0c365b117" },
                { "kn", "73e8ca5e44806baf1a686b837db98d5e2860ac66f76b22e51ca3091f167211732dbfa2340b5acef889661a846832be511bb22bb4e656ca90fc1ef459603b2350" },
                { "ko", "029e420fc5277edf21f70db554dc95b69e343c4c598f0bd9189dca06a1fb523b8657444f1b88bebfd2bfeebc56969b6511f9c4059cc2f28d09feb581eb527c86" },
                { "lij", "a39b8ffc082f9de0a7eb745049f51fea5b5d3e89f6553eea48ebcb8c385a329a6c37eaef335a2f77d0cf01a7aabfc22ceed05f719b821f6e4bff855e2821d2c4" },
                { "lt", "4f1f5b68bc8fa419a54c8c904831f22eb2328a671fe37fef3166c9054d42b2c5979799d164662cebf8e4d073df6f2f2c76825aad758906e2dd3761d25f080d57" },
                { "lv", "f68922905c8e7d11da2c3c3fd2232771350b7babb02fbb10c9f28d7cf23243020e42c33b95a79ddee840edd231cc7d050ec65b70de9ce8c49c33080de24b8b75" },
                { "mk", "200b589274a04503b7ae959785be0bb8a761570beb71595bcaeb255729ca7d017a2c9dced3a2ba5e029059da809c0939589f39c652db717453b4b175d8029543" },
                { "mr", "cd4e8a1f33890ba34abbe44baca354c674468c3adde000ce0136ad605a8b186576f18001a883a2525f24af442756de4a63bbcec58fdc32e9a257860c2d4dc449" },
                { "ms", "78f055e027783351723d61ebcc4bb693541a468aab286b9ee778e69a3464cbd731c616617776124a20ab59ab077d6e7790e8713fa8d54bc5ced0fe5d01d23188" },
                { "my", "c8d0c2488303c5eef34e6cc55963bec698f259469a1565349d5ea7e05fb314e47e35013058dcb6f9861feb456b9900e0a3e3513a6695c85efe7a538a6d92e5bb" },
                { "nb-NO", "078274d574e0fdeef890e88f202383b8b3b8791efa6303cf35ac2a03bff874114e5be0d89cbfe76d3fd43e420b8f3eda6d03e39aec9cb123edb82c02c9d4cd57" },
                { "ne-NP", "ccf453810c9fec0f6c747e56ed386235fae862f77d8f9d1b81d6e6e44b72b122913b0875497ff78bcda2249f21c683e35ef88ac95117d8076965332128dafaf3" },
                { "nl", "9e9bceaa4b6c6dbece49bf968653bd21bed62d23d09290a773ee936056b1846b650a85d9a37fd536c93114b5691c1a44e4b76fa410c26332d20a66b3ae5e86d6" },
                { "nn-NO", "7ee1b43d8c616919ebbf95b741496bd06a0e0f1e016e3a22a37b1ce5a4026db21c57b8e4054ed2b194acc874e046c188b02eed239d7a0a5329066e21be7ea3f4" },
                { "oc", "8c28e68229ab2654956e69cb1b65a9596492455f8d321baa3364630674510d2c3a7bcb42a30d4ca4d59d5cf7a99af847528c60df0110b4c62ff7d36bf02bffc3" },
                { "pa-IN", "02ef894623985f1681b16ab4498b306fe6fb2f2ece3206c9ff4293647b932b653b3f6739c1bd023686b74101110af34fedab192d97fd12950a72d7eddbe671d6" },
                { "pl", "9f2d0826cd4ff295ad08c84b25ddeefcf1e411fceabeaaba0d292cb5ae65b433123a626ec8ff209b263f6e1669829e5e3ca98cd0d3342e589f18e6b2186b7b44" },
                { "pt-BR", "11fec43c58f8c9ededb2170cda45bdebcf382c8a3d634be3cd5db9de8f911e82d2dda548c52bdfa01d689cbd389ee567a0f543b4eb07449fb08b9d70973bd8b2" },
                { "pt-PT", "4d2dd9c40b2743179f4f91654fd594300f8df7518e96e7491dadc541e5407cf93c7927a8fa1c5391e80fd39eaf050c7d1ddf6045963fba83381d6d57f513ceb0" },
                { "rm", "f9d996fac2ee51772848b7e146a8edb8ebacfa9a356b1b6419abc0f57912e5cc03232fa90daaf50677c0991db0aa6fcac135a3f57a5aebfb60419071e0aba718" },
                { "ro", "ff54c8ee85275e8d2abdc3ef45b4d3d46d4d4a1ecc31d8a86997b1d502693014f7593fa6e0b5ab002fcfb309e21b6b99ddd03d83f704dd6eb220ce2e9e48094c" },
                { "ru", "b3bf12aab09af58b969b823d00b200e73ed252dc6295f641401ee9968fab6f97df2baa6e0c05c9386508ef7c794631493785f6858256055d9f384603e26645e2" },
                { "sc", "bae950992f8ff8da0fb4ce8a98e25ce3f741b9d3e47409f454d9433518f3032e9d3cdfc558e5518cf7e5d4fb289ac27245a737e8e127fc8437082648567b359e" },
                { "sco", "6e20e6d8929dfbcc256863f103113c4a08024bf187b3add7a82b90108faf05cb56044558e9e7cdd8e95936c7416880be51ac9b927a6a230460bdccc024fa0fde" },
                { "si", "532d6369ec4dde4664b03fd0658a4e1e5b87ae5d19057cb629c0f555097362c47c9bb909bfaa3185b8fc37f03c265e74b781e872cb60f992d5e42280a5401045" },
                { "sk", "58bf3b77968e1df29a25fcb6f78f9bcff6648b4344dcd260799d7142c2fb6c423388ea0defa71998d8ddaf560f95cd503809aae7cb0ebc8cabb8f88699a0d2ce" },
                { "sl", "3d4f432dde1bd03f9109f2a38315a7c29f869ae09928b5d4371e1976c58dcb01bbef4b266f5761fe0eaa8398d2e23ee668bb2e1980bd52134b633d2d82276746" },
                { "son", "d55fc190a7fbda29c59c436ed08402a3f561cc8e6301f686aacebf252498f9115e6b9a4230a1dc0da4b80f15dc54d2727db637e27f19e21982b7638152a13776" },
                { "sq", "9e5e6ed0b3783f1ca45be1364d1c67308d0e1e2c77e865d1cc4ea1dc4006ba907b3403bd6f9f04a41050cecea06e81d57120989429d4eaa279f61ee22a389097" },
                { "sr", "b1132f64b4b2af713bfef39741788412520469bf21ff1666c86da43ea436f2d9990b8929838dac3c24204166057702f9d9d3c07d97fff1e8f2ac0808aff69cfe" },
                { "sv-SE", "b0a09f3fdfc5b0dd8a524c35beee183a05e85dec6bdb478e2f5e91b254d033a00fac00b7feeb5551422d9a6621449f4ddb45e6816e149706a17c60887aa01f0f" },
                { "szl", "01aaad05814158317ce397a1a251062800c9c07aff05201d7c70d9a757e5a48934da15fb9d3aabea63b77be6bb900504fa1b60954308c1f616d44edce5bf95f8" },
                { "ta", "a75af6bd2728b18dee5fc60cc6690e4f424052d1965235e6eb22a5cfed6582a99360bb50d21d5e31b100821fa0a49697111e1ffccffea0fc5fe7de59d60dc466" },
                { "te", "b370f2cf61116315df225093278b69c809d0f07ee5a1a403309506d6f0395f7580667db750ad0a0dd5a5e297196eec3ab464ad059c9db5cabad71ca5addca92c" },
                { "tg", "78f0088da73cc7e373495299a458ed0a904222c9288e5295fccabed924d3922b861a7b5b9b325cae814850b945336b71a91a92609d74cd6e198b09b9ded91c74" },
                { "th", "ff89c257db9e8fd590ae1cc0ce9a76e5e2cafa03c309fadef43abcc33a4c8cdc3b429eea1ebed20e709b1192dc833030cd23c5cd7c27727fb0d71dc2cbcd3ad8" },
                { "tl", "2293bbb9b453567895c22680642ffd07da26c1bc6a7a5870c73037f834aef07330b363874953659c72e70c42beae92e238d113dc03f1462a8aa5933ac4730482" },
                { "tr", "e848b35f8a967bf780b8b78d1a116a55fac53afdb059cf6ecccba0444f0c1b1cc8a77e631c7bf1a0c030b29f6d3e45ef6a5a90fcaf9fb1b2569008806e7ff368" },
                { "trs", "b5c61dcb8496bc12846e1d6cc039e5c547c6a9502d2091b6e84a4bd682b09fa78dece620888b1a623995edbbaca1bc8212f8441bb74b9e66487166b93ac9f4f8" },
                { "uk", "17b01702010ef88bc4236fccf2997a099a73704813f0506297b2b33fb061f7ae46a9fd074d4e12ae6bc7eff80d560b4e0a375c3ce4c71d8b2b080e020c165875" },
                { "ur", "cc8d3438016354704c2a0aeaf6306fdee21a9096b800b7fc8aa87ca7b96faed9d9d13359ef42d720cd4e8be6acb4824f7442b78917cc280155a2bfd498518fef" },
                { "uz", "86b912aee7efbe5ecae935e60434697d4da8b0521a89d0479becb5800973acf06685e0fd3b7a5c7861516bce45ae0ce6c8ce7b3e8b00882370999c0a56ad6520" },
                { "vi", "d60756a191294c3b464d173edceb889ced35fc2acb586f0dab9258e4271365746fa63e3d3e861969411d777e6c50e3ba0d63dbd54eab4b2b0e11b151f35fcec4" },
                { "xh", "efc5e540ea672116d91fbde7062533e66000bec603ffdeadc10cfff5e55b6a017985643dd160e00ee4d8e5b6535f06fb9ff119211926465e04b99199144afd18" },
                { "zh-CN", "0bfb3a33fe99f2c81031e7b963d2848a2be8fb63810a9da5424a7a2374818720e7096c61d3525728ef597e7f2bd1fb050ae258bd4def370a1f90f3e86bbfa592" },
                { "zh-TW", "7f42001c86f556c77653bf154fa5eb6c81544bccb29adbecf703b6cddc8236396f9c69402f1fd3faf0af8ad38ca7d442f492e5a4f60441aa9b82fb4a89a38d5b" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/115.0.2/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "8aed56e6bbe1342215c3e8f010bb079d64eb77237f287c10ccc3da22a3efc69b64f6b6dba5e4097cb926ccb2c5a5204775cdef37dc36a42700670ec0f61580ec" },
                { "af", "0639099b0490fd6f1f19ae58483d9f460962fd0c893813a2e93b53bcc5f35ba8e672b61698b579cf18abd7cae61af0a6a9a4febddd58faf46b29955db3022929" },
                { "an", "2fc0e2032106994c3e860f0b88bad047a2a326e01da67c9fd552ff90156563d6b98f1d62fa974d910194f204de26f8afa6f2e5f60f7cea31fdd6194128e24b18" },
                { "ar", "9be1d9bc561b5d91efd569935f3710994fcc46f37a334a9a1807ff3c448cd9e3b25a5d717ccf01a089155715232a4d2e0060ddbce92e5574ebf187477cd2bfb8" },
                { "ast", "15ebac131c5c2099161fff99dab696243acf216dd95b04531e74e1bede841601ff5f0c647aa21156b45bd948bb388814b5df06a0a98fe0a27de80f1b0c12b4d5" },
                { "az", "dcbf9bd4d68dde83339eb5278a10dff532c5cf81e43004371c84c03489c55403e95dc901a98e345781c2ed979491574a496795247bdcaee30962ee604257dcf3" },
                { "be", "e12e69ead6dd365fa55cef78abb54959177d2eb2513ab0edae5798e54d865b4e94a93ca6d6465d56f8399b68258e12d05fd0299010f4a535a0304fd1072b683f" },
                { "bg", "8df58fa27158bca50e3c0423bd9fb7e922a81408a95f65b78b3a4e52f33558b188e40d62d31fe845e913c72de66a8043f3c72e2f35fc51b23d7720ef54ad0463" },
                { "bn", "ed215546091e09bce184d395b765ea99ba070abd72249eeb03b2b6029d461f7e728a7f86568893838b3de190885d81bdfd9e1686b202a5a8335e00bee67d8b25" },
                { "br", "98db20087b9b01515be0ffe5a557db795de45435e61702befccbde8176995205b2e59a9fc5e0f77fc5f246a140bd70ef983c5e8b08ca274a6d92c385661ad073" },
                { "bs", "6bfd6bb4d1fde619f33dcb096189b03124600b9a9c80bd8eb65e7d5dec1884ca106bcde7a4f1ff5d1d2115e15cd783131d61219efa5b83e07984d950ec0b2c5d" },
                { "ca", "38d0e1ecf5e20472975b7005537f3f1f5756de94fcfed51b6c1041426675c055946f403e3832df43b1455f4ad2933774dc95e8b83b7edff4042df7efa4b309dd" },
                { "cak", "b2701dac4669b941d113cf0dc739f38469d3f2a2c5cc2fd614f58c38d5cd1f685ad91417820e8f2735a6534147098c3ff2bebe479031c1bcaf93869d50946cf8" },
                { "cs", "cce12b866b2f4484f2cd8b59685a780b35e719a8819b5b65e56407dc09848ec0f126c60e2231aa78c8ef8be2cd7716dcdff47dc9252477b3f20680e50fa9ac99" },
                { "cy", "4a6b95eb0afae30e2061d20cce3b0916517cc984e97bf5b66ddc4c49e911ba19d7e7cbaaaea5b89c7370b65abcf5f56e7083b53cffb6685ddedbbc0c6d47986e" },
                { "da", "94221bb3e0db7ce31bcef5b9a7121a1380b3177de06539dd9a72c83c970932320e420cdb58c583d121bfd50058a103f52f5d97a9c8d95028e9f83f2162d904ab" },
                { "de", "3b2e6005cfa45a2a0a2cbd73e9e2dc75d6a38eae99495e51ea24400b4f52ffb71d607dd32cbda2bf03ac6b2610538c83a23286b8d42ab640175db5cedfc59be0" },
                { "dsb", "d707b9965cda972064118f40d652331c297879a271795451fc6df42abf43a95a92341eb0ae6c8ff117e7a676fdd81dd8e6cd1d8d53f8c496f1d4d0d6d1217684" },
                { "el", "f7d77c5afb0753fe0dc2d8590c7c5eaaf034acd0d72b01ef2add0ce96a23d2a71adf58f1e1a38a9e53c461a977aa0989eb1f8675021bdd1daf14258bfb903f1e" },
                { "en-CA", "14a697aff4b18c181c77a1eb3225ff805ec8417c484f67931bf7a44a69ba6171d97129085201a83d94b54ca2cda41047e7c25a9eecfb370af1f6fc6b9ebb06c1" },
                { "en-GB", "e93a6e9afc695a8dcf22ac7c5ec6d09afa20dd13c992fa0314eed9297a6404b84a8012f539ce5bb0f54e0b1191d34129886b1fc708a72ba830475f9a3a24f090" },
                { "en-US", "9f54b3aa1d784c6dd7529986ab81864393c015019c368687f71f407752aab2221d068ee68f5d7b236deda8b8e14ec3154e744fb7ddddad194b86cd03011342da" },
                { "eo", "4d44275b215f88947579293e09b2c572e04b1ad85f6ee17b3528e7468999a7c325fb462bf9984b3c8b5d5cfd1071b6c879e34e0ccd640c781ecba14f122119cd" },
                { "es-AR", "2edfa30d53858407999c6e22146d391230055f56466d13848b4edc609b3856eded100f4a281acfa009da6e586223d374556a2e02d6ac9b34ce31c5b107a8ca9a" },
                { "es-CL", "b9e6e77b6f906faa375580e6837b6e280631ab82be14ae4653536bbbceef42237be142e2ad6fc9ef96aa9a7314490cf305f5d88484579ec7daa30a2a82cf0e14" },
                { "es-ES", "025d79725dc9284b6ae4661a28da68c67b836dc379e3d81ecafb705355839d38c5b75f4e1285f4ac75b60124b1eb1d6916dec996de90ec40d122fa8080611d02" },
                { "es-MX", "3726b46d72ce32649acdf1196ed4899dc6bd9f34bf657d8de5ef1ebb7facf33e80a33604bd0cfe4c7342b43c6021336df147a73b66adc8f97a44515915cf2798" },
                { "et", "7d72619c8ab2f266c6e894553a57bf0f3a4ad1d18e769d4314933f021495bc811fd05ef05a62388d89af1c67c741c9f1cc5aa6b9ba470256b438133befe70451" },
                { "eu", "21ca1f733cb8aeaea725628f566663185f34fe044d249b310566dd98a535609a9f6c138f8943d6c12d715d9ee3abac2d293c976f0ae0c9ff6ea1eaa2391fedf1" },
                { "fa", "fa9f8fafe032f10a4718c977a5b0c9d0242112143b83a25e61df14c3160840ca27506074e123e93e6a5cc23aeab2e614c5e855b4565c5476579592e6a1fc7e9b" },
                { "ff", "b856495a141f902a9dbce58b3d2cb183f7a21ff881d7d2a7d08f03ec6af271467375be5af75209b619ff1281777796a95efdb955c8918a38d6f6716ad83f6bd1" },
                { "fi", "0925e6fbd2b84d6cabc41714020c602eef21a9bb6148efbc10f97063c5af28249d121136f19a37a670778699808682f56cbebb68e8ee850a530fc1ecc002ce7a" },
                { "fr", "6ca2ede418fefb940f62ba3c8bace850954730766a8e9adb86cb4769867fcbe83cc0e5c170eb0a4eaa7298a437c5f43ca9055c09014c3d7a52a54c588aa721f7" },
                { "fur", "908a54b4205b8fb1fd830e9c1ec28c45c965da86366d99b0115a773d1fd5b377633939e4c78538e2bff2ef4b595c0ffb5c13150a662a49c5ac65549559afe80f" },
                { "fy-NL", "5c890dce9953e2a491d27b7f4be30f43bd58b053dd5f30cce5411066770e0b123c9d500f218b2e9c0ae7fffb1041e061cca086a3596e21fabc73eb8e9b92f1cd" },
                { "ga-IE", "25098e53e80ef8bcc1d8cb53977a5b767856d0ff8ffa0125f4e1ee2f080b10eed5ce0cc5322d3e2efe49d287436b1b2c03b1e41a3d31a25bd2b31f25e261cd78" },
                { "gd", "b7ca633dbdb84d9e70b63ea67e21a1b861a478cf7c2b8511ec79e393eb99f36243829a0a9b8ad6ba7441de07444037a3b2e5edf0a4c9bb11e97effff5d6c3cec" },
                { "gl", "cdfbd8348ac3d2434e3ec75ab29eed93810f00391667b9d46e20007d9c3009c8265f909d6ab700b0a8bb047800697597b6a7fe798fbbfbcf3e4bbcc22deed46a" },
                { "gn", "6bfca2c454525bcdd50f9dc7fc0003ccf870c62e1c178b6b7f46791567d241f8e7ba07f1a2545fba989dbdb1529289737f9e3fc5f3bc08f640f6e4004f9d4bed" },
                { "gu-IN", "6a7124bd0126a5206cb354eef41dc24ab412e74840f424f96e3f4610bf3169136864dab2979c4d6e716809a25d0fc461f791fae60898b9b61ff620df3ef484bc" },
                { "he", "a07f1ac07fdb68c2bc56d9556387f27496d141e4b59826f70a02c57168e45ea8fa0863ef7e2012f231d155146fd77c657608e024b5a0bdfa259f657dfa89d890" },
                { "hi-IN", "1e92a23c3b7a5e00e69d51614e45add7191041d71a259e89bf1fe4856b46c80081685671cd94b6fac0b2f1059027de582591f3aef90c682eba8d9a83a81a34ef" },
                { "hr", "ad0c4b0a21d5ed80395f5b637558d74f7a3360c1f0129e5614b623d5165b10f8bbb3b0dccf58b8b7375ce82e38372cf441c823a741047e23b9ec93398c7f48d1" },
                { "hsb", "cb9d10e8b97fae8150b90ce52fb8ed5038113909204c06dab81a3f1582c454ce0a05f0754aaacf2e3b36343db61d01852078ad9de4bfa2f509c57141e7a5fb0e" },
                { "hu", "a4e9bc26b2ad698ddad23f0b86fd5a12bf482b40a1c86e24518998672d2b01ef03f31bd24a835cd4206aa64b1f6351723c0d3f8ff8548591a6e33259493fe568" },
                { "hy-AM", "67ae1c9bdbcc7511e33fdbbe9d384881284e763a4672865e66ee8351ce68a5b950a67ca630ce9c5b4eebb01a8bf8e4861448bf99ba9ed52ed2666fe79028154c" },
                { "ia", "692a2664fda70791d77f9248750e5c57db3b2f6a45ed875ce9b239e257b590d01a8416149c8dfc442aa0ab401a0e787e0f9b648222c17de2c013680a93daa296" },
                { "id", "acc13cdb3a836590f98fb854365b7c34de7ae8f82957b14621fa11ca7a71f4d84de6942e3f806c251ddea210243954ef5c3123ec6dd8e4113ce7bfce434578c4" },
                { "is", "df1d2d2a38a1bbd2778e49a143f8d1c0e435a0c6feec9268050714590dc7bc929003c35d78ba00e27a3bf5db9b80ddd75c6bf6a250ab39417e553841291a6361" },
                { "it", "2ca4b75d12b7760cc027d40186cbb35874cb695332cd84a59307ca5692e0426e04fddf9212c6445328eb1efef5680af6210abcdeb414a5bdef62e8210b48d0d4" },
                { "ja", "572ad2ede088621a82e8adeb51621fd6480b29239af1c9080a071d723ef62877a27f0a93ccbdd5b5894f367a41fcd5ccf4188089def819fdf6ccfb64fbf8a1a8" },
                { "ka", "ef38c84cc3987f1581f138abf1faf9f7cc7ba74a7b0f7ad7ff7e8518b23af5086365f4f1229a04ac794a40208715a14f8f63bd328558aa2695f94aeb8f60de54" },
                { "kab", "bb94c9628ec49d5c5d0e229ddcb592583bf09f598e15e239957cc6086d49aadb124eaf4d940ab893133242335518166b145d4cb03caf96fea777e01c7d2ba34e" },
                { "kk", "b197d98811b29360bdaac3f8887efa8a7c0123c9d8d1b197e192351cf4ecd86e5f31bf0662b67f5042926385e295aa0d9924ade2aa731d94a87cda57d48b4266" },
                { "km", "ac52f24359ad2bc56e47181bec404b47e31fe9ed0092b9a6fd1f0f97f78c92df7a1b31fd3e0afeb6ea0fb3f84c06e1170649c36e7854ad86ce740afcbf890b06" },
                { "kn", "456407ff88ed1965a4b9cbe79abcc39c5f1d505066b2556e14eafd23a1b1a5940081207e7f711da0e09e13811add4b00e6b6e1107b1c1576af41545331400b5e" },
                { "ko", "9a01ae4b81f19879071fb82bf5e6413e6703360a8349c19d47fea1e2e08c2a916352d0b63a23cf95915e2e86c7c1951dd4cf7f29abded5a975d156fcf3cb21d3" },
                { "lij", "42135fa243b9a09b7ca5549efb8c1b39943a57c9cb645e2564e6404ace3ebebd5c3a82014d43ca42dce16bc25de942452794185c918e8f58f5166d98409078d0" },
                { "lt", "73072262ae9bc2e17077c55afe9581c4c8e348368db1c70f0886d258b2a78aa004e041900a63d778c2c43931173e6e3df80be4ecf482a12b33751e0b34b29114" },
                { "lv", "46d0cf98e7afb9b59248b4b4ae578f6e2efca204e2ff6f39d5cb81aecb5dcc8a4b68f1db005a9e5adf18fdbc66b981faf8c4e741f0bda922efd1d6f14dbd7fae" },
                { "mk", "80a8e677284049eb7482a7499e267f983d5dce91d8a4653e267f2e233e16ed2cd57f0cc9955af39a13af8fd3acf56cb7098edcd2bd6ba471ae7ff31133b1b171" },
                { "mr", "0764fe32600962fda2cbb02edd1c513bedfcdf049b115ae479105aff82f3c350fa00666dcb0d72b25ccb12378bf764a9af9fae818b4ea461a6a381dcbbd56876" },
                { "ms", "0b250ef2ab787472334151d4b054ceba59cebf2a08ecf90cd3ab967f1533d7a83ec61dd809455d3c8b26cac127e31b14d230c9cee23ef3880ec8a9c946335150" },
                { "my", "e092e9bcc54bc693f1a7142ecfa0ff501b7a9858988c417b832778fdf00336f1df3842c8cbee9c54f38e85c06f887f0a3f1f5f3a0d5d81d4dad9aef14e2d8c2e" },
                { "nb-NO", "6e15b26f073ea96a12807d5398392b6462a73c5bc61509b66def400ff1477ad597c3a8cd10afabc68b05ba77e4ca46cc9709f1f465de3f68838ba2de0554e26f" },
                { "ne-NP", "ad3aff86314547afcbc9775ae89d41b22aa2b660c55ed4b98df4953ccfe21493c762c3f53b6f591977443c5911ca0de0b4eb68e310942426c0975b60620e9d53" },
                { "nl", "ee2bea34b33f774e48b10bba3093db35547e5bbdf65c060565d534b3a87c79858c1434ba9a6d25db04cdfcbe2ee2612144aea3dbfd6e3c8c2fc2ddded9b4d278" },
                { "nn-NO", "debb98a55f60b197b74011da3e315ec4738e212c59a4842abfdb3a5536ee958d644ad1cbcd7a0ef9ea57c0adeb1d2194a9d2b3f996a5a1bedc6af24eb193e86a" },
                { "oc", "b2ec6545ea4e7f62da4d0437afc9440be42fecfc59c4e663c9f364cfd33bafef20f4baf8685c05bcb3d44c6c516eb5086bd0abd11b5e8a589b6ae211a1661909" },
                { "pa-IN", "8520791548655df8cb42612ff951d892e613ed165bf831cc55b86995ec475bbc47648ad6834c01a14e4488c567298540d5b63b3bb86ddf638741b9e55a099892" },
                { "pl", "a1d5f2ff9538dde15c2181a12e2700e146009b977552bfdd0293654aafbbdf9ea9181031005ffb3870bb778d5a5d8171c8b0b246f9a5f0ff4c913d167e3dcdf6" },
                { "pt-BR", "ae5b6bf3ff3fbf1732b2257d8ef42067e4d1bc6ece74c883a3258efe74a24a18207d11bce2bacae993fa611919886236eedf736a05f88c496638d0879c261361" },
                { "pt-PT", "958d34e01fc0970cefa924a949bc7260a636b19550851df6ba9e196c8e878d613bf6c83dd3a6842a3f2c19e0255d1d8236b985667534bf93a2499a51841d21a6" },
                { "rm", "c292096093101705dc31f50cdff46e366b5967e85f64396f855f8e4e1392eae3f1a945054c6a7f7b884b7a53781e759ce04c4ef72d9e23516c8198bd40bb8799" },
                { "ro", "b606761656b5e0db009ed978e60efda7266a80dc29db42b29714a5f7bb84bb955ccf8b369e3a84875a1f90c4ed5c06105e735008e856a0eb71e64938e6f084ca" },
                { "ru", "3172484c7b8b33c6957114bf73769037f0354ed3ffcddd04c214bf55b5ce93c7233b24a05e721805f73229d2fa441bc00d2ce1ccf56b698842904b2d0771c409" },
                { "sc", "cc20c500caa4902003113dcf860c25be413a3e1dacdf98a9ff1731857d5348cd8a0a2bf85738392379604f37b01a79d7dfa349a201817420070d8a89c57caea2" },
                { "sco", "0f179be95a6ad536fa817196bb068a8623979a440cf2354f326a6b4a08a0047bfec7d19615d1943ff3bd2efbd703072bc0f3e78e6215bd74eaf8eda8442975f0" },
                { "si", "2bbd12e83aad024bcb717d49a3517ece016900a42de28c13efa628964ef95319c8ee0f701328ab92a50677fdff33ea5a9bbad0c6036940a0c55fcf0816ac9dc0" },
                { "sk", "ab921fe77c35cf05391b7072062e419d2dcdbdab5e9984d0a082e2d33cc1a42dbcfbef929706f995c38065d6aa702953fc6ef2569cc26f7ef15d75c3bd8f14c6" },
                { "sl", "09b3494a2eadb4796fc44372dca3e826d85406abfeb404df19d45e89e9621b67e722f67076c59be1ec6873b02a4ba67c3da527b8286c20d0c84ab6689ec0f25c" },
                { "son", "faef41a628267a99c2b24f800be228aebd04276330aec577cd4aedf930ae6e2f4f02758575979b97b8ff2623d5682e49c840c22295da735b3aad8c7d3401e1f9" },
                { "sq", "2fdc1f7db2254dbd913bd596c13c29aed39efcec505fb1a02100dc5f14023317160df852955f23564036d12985b91d971467c3b24822bd59328f69c64ef48982" },
                { "sr", "6158797c0a864c710e60f26f5ec46968c03c8aac5088c6fee224905b2718f7845a854cd8058e054037c598acfcefa521ca9a084983a774592ae87ad69cadf95b" },
                { "sv-SE", "0a0e3d45ca1c469e504743a105944aae98e5c1f592505c2a20b20bd1ac0df181d1845d5b90031a24d1deea6cbb7d879d66fc9d825b17eec88bb96f0e57bb516a" },
                { "szl", "1e575cbf8e161d18f88083c1c2a34435b5222dcaad29b2b257924130f31cb950a7b6cfef44c1bb9586131df536c61c33e8c45ce884e4d8c49747529bf41ef262" },
                { "ta", "41f8ca042ccfafd0f45bcebbbba916e12335d99d149d374106daf06c3ac21d343d7a344f2c61ad5990ee47da8e103fe204ff9a9a76f3c79e676c8d3c25db76e5" },
                { "te", "4ae3c11d01dd44d20d69e19b52b691823eeda30960683eb9d2f523f7804f7cda8554b2f9c25977a8ebb5f54215dae02ce5a60ce29b072d75f985ca4eed0b18cb" },
                { "tg", "7a70690215d725813a8da601536b465cf1fa4eb969df839ebbb5295080d97a549d62242e63feb3844dcbb39adb640b3f0df49ca21ec10dd5b8abb0387841de38" },
                { "th", "46d9ae6faca10688346a242187d439a19b3c1b279a82c56f489639b5961d39dd36440d0fc0871245ec9197bdad49c7d048c37113159de062fd771331a2f06b5d" },
                { "tl", "28b442fc014713e307eed0bedb9d1da72d69b3401a711dab875f3690da4c7e83d900d18169ddd78b7af7d9acda1881d417fb40bdf6c7106be6371012b5516cbe" },
                { "tr", "0844a3d9d8995e671a1e5ecd958fc9ad060c9772a00611f45a44450971088831a1adbb4ee88f89cae44f41a5b85533e16021f0905399271753b40f25e7d7fe81" },
                { "trs", "8ca52a7be2761c7020880cea33ff406583a1d8372749dd012bb908607cde558f19510a32fc20647da08f1ec1fe908175d53b764c08c0d93cdde580a59ff98fb2" },
                { "uk", "3f5eff337fca9626ddbe202472a41437ebf09cf8be61521f077f6dbe119e0aa333745ac9bbce72664c21897a3d51a040c219df9cfe5745a2db2a39171ea4641a" },
                { "ur", "cb711c2b6f5b2fab48cf266efc67a07ee6033ebab8c3f751db4d1f82ed46b920afa4777c9faa2f3a8bc3b504b2aa4f1ef4c5553bb6de8af6f2fc2509a9d3ae01" },
                { "uz", "e7cc4cea3c21172767b85c456fd94f8001206bfd09e7f2a6689941a6e60c4bb9cbf6d914eceb2adea3ac91b39573713a6bd0d7a431e6b6d9dea8a55de9b3ff4d" },
                { "vi", "1266aa77d657b47aaf5be2751257e5dc45fa807a1dd9d4198b14698719be7d406ae088fb9233d45129389259a0c2105acb21ae65344eb5b2c1a971237c7e5ac2" },
                { "xh", "7c89e2b4eb334aad1a0bb10075a7461c5acef5365d8fe0d05aba8c8dc476d99cd4f0ab6ec2ea3ef38f10f50ea3c28a91bed689f3e485b9296e3e0ea159d6b692" },
                { "zh-CN", "7d4e47dc089c77347b8b46a8b62847e124ebbd8172d5637f411108c289d57bc12807da6fb72ce8b7151534f1a11c01e2d9263356a6001a5f376df3be7b943480" },
                { "zh-TW", "f06ff6075fc358faed2599cfce163c7460a580ed5165043d9d8ad0d2d32e0beecbf79e73846b3c829b9ac4c03e8d3e8c23cb00ad2fe18dead44f319f3eef1d4f" }
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
            const string knownVersion = "115.0.2";
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
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            try
            {
                var task = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers.Location?.ToString();
                response = null;
                client = null;
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
            return new string[] { matchChecksum32Bit.Value[..128], matchChecksum64Bit.Value[..128] };
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
