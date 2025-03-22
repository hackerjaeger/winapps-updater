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
        private const string currentVersion = "137.0b9";


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
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b9/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "469c0ac21d703cdc04300779c90eb2e2f6c229ca4b3dff6512a627f837d45a7a10a8b8cadb1d74987eecbdc7f812ebdf03304eba0f8363e8a352cec0ff92d05d" },
                { "af", "34531c492c1877f497b7a96f461d257addb35db8e4b1e0a0158a43f5623b281b94a0987ed7c6472cc27c1bba48b34e7d5909d10a5e3189d21a7041b5b63e9040" },
                { "an", "f4eb504cd3ef292f097bf05b44ff0e4dff3a407984107bf65fbede2a04f1cf5c84c6cd7d0dd74e3138a5a57486979069f5572f5e20beee4f63504d8b1aaa525e" },
                { "ar", "6e7b0f77f7dc837e7377c0b3dbbee64de2df5a9f24f0222e42bc48990c3dcf5b43e53718e4b3ccfecc75a25c019f82a07a5788b36eb0ccb21ab749bc6a466fc2" },
                { "ast", "6d0c295c4ae15ac4f91c8ceebd8a50fb22cc9b4198746c163a229408684a3432fff20a5126f7afca35c8d0bcc7112fce941b9d1cec585c115696bdaa93ee3152" },
                { "az", "c11bdf5830d515b0e62801bca4f01ff1aa72e2b3172e70e7272bac15a57440ccea1a62a7cb91f007f7e127c2ce6e3460e8d4c5f2a4167f8f1a870c3e5c13e53a" },
                { "be", "f7a54c143c579f41b111e32f9f20eb5baa9c002d30c84e9000488eb41ea3df93be4d6a75ba32ce8fd720ef98687a1ef65568a7b3aff33e76d39e172a6a13a518" },
                { "bg", "8a775edc4b876ff1151024151ad702eacd174da95da0316a0c836396224ce1d735437889c3804d09c6ffc4467d2b118ebc9946880f7817a99b06147db1bd82da" },
                { "bn", "dcbd37948fe8643aad18a217401d0b585961278e768a6c85b0f16f2fc8204225115ccc74907caaad8d30184a371ef54fa3f926d952e08cea00450740ca88c545" },
                { "br", "6312c46b1c81a8685a55752ded07293b324e99fbe8a4c67b395ff43da03fd4bcb9eca596979d2e218f458187824a67e907b4aa13a114093f67476213d9e60123" },
                { "bs", "3c14c87f3554b81fa7f412bd1dd92e22409641546c6756cf6b1336699bed63d04774ad8571ba3a1c5347ffc9511c2a1bccbe57258f382111e1f8b0e4d0d4d9ad" },
                { "ca", "a88943d97351a8f8bcc4b20c55812e3d54c17958c9b85a6e213e9ce17633f4d3ce507a43df23fb5d215e07fd1eb6eb6e10f9c6773acbed78ebaeda2b9b376525" },
                { "cak", "9560523dcb9de84a2af0aa9e21e6f295a611d4f4ecfd891f867d2c058f964ce160cf48dfa70807f20acc3e608664a4ac0f6ee222535537c47863d5e78c010863" },
                { "cs", "d44cde60338962b49f84a3be350986475dce1ddfcf50c635f14ca3d89c08cbc3edaf34edca1bebdc517866ee164a80501e8479e9ce7ee4e6bcaeefe80c3cac11" },
                { "cy", "02e61be711555199f214cd3bb665b455b6cd817847fb658870cb0980ff40685a56ee216c30be790ef00c8622fe0e848ae8e2567da66bc5aeffbf2efbff38c81e" },
                { "da", "f3c88709bea381e03f48205cb4b9a4e1f55147ca09cac3a5e235205d3ee73c987eb1b4f31f37fb8e9768afeeb3737fa738cce4b6b1bc76c756d244a6085f9f26" },
                { "de", "e9cba309b1d225ed4ce6700839c8ba758830bf421b273096d1fb83cccca154abb75462ef00e34e2e148b43212fd73ab69958120022fbf9241c71f90ba080a553" },
                { "dsb", "3e09578abe46abe445f941505a92078c30bb8bba48bb0a8c6732e3225f8341e0a530a379db05a36720a5ebd0bd802edaf689c09b5dba617d95286887d66d5d5b" },
                { "el", "40f5b1735a559510d4edbccfcafed62708ff4f3fdb784248a8f47b4b837369ba72f1656cec787119c40ad6846839eb0cd4303ca473fed12a59f14417ad50da1a" },
                { "en-CA", "9155e362fa7bd82ab3232008662fb8a1e3fdb891f174271057f928b601c2511d42deb3a6a4d4842c6f5376291dd7af99c050f4e00407d7df0f99a35ad23a58b5" },
                { "en-GB", "ee98d8747519250ac8e0f36fb5292850eea59de5777585f53b500c7487ea1c2228c02e701ec14b2023acf67bb7f6e223404d1a172774e67f737dc54555bd4c15" },
                { "en-US", "7e1ef63852962f52aa092926681cc0f0d01f3e6fd08c079777b5856e8fa31e2a11b5fcdb94340c5dc1cabadf5729ca0465260e00673a8a880d92fe7a49737569" },
                { "eo", "b53de4a80c613e5d5ef21ba43f6c01650bce2982df4d326b3730d30bd70d3f9f3db94a85b462bcdd0071360c51053ca3288a02a8ebf7f123c69fbebdf5774c20" },
                { "es-AR", "c089123e79fb48c3b48d9f28cf3a1191999e886aadded12412dfa4887194030b029ee5e2d1bba5e64070cb19e4e48a2ba2de22378ab868c759f0f1858acbb6a7" },
                { "es-CL", "d86a50dcacaa7c333d71a538e17fbd5afb11ca8ba630e33bd60abf14aff11439cff7bede35208d1a26fb11e00068546eb83aafb9c0373f21ad83de3dcde27fc4" },
                { "es-ES", "b85f079bb44b31673be81b682849a1bd953585ca708dedf0b07367c28fcf75c619cb547c202d1dfde8b746745a0768e73075da6d976e1f11ac0b2e41f0608a28" },
                { "es-MX", "b507b5d376e335e6322a1e919bc1c17f16edf06d1e7f5bec31d9ae5cfe318035eb65cd9f28c681bce7d26b056cab3f256c703efdcef7f22f7bb1373bc2a4cb3c" },
                { "et", "7a1eb71bd273b28574bcaa4265dc562db02bb8afc92ebc3d8f81591f7f7e338b8fbfaf6665d6ec8671c4524d638cd11fd7bca8d3a92e1713a2f2244397243bfe" },
                { "eu", "62b338d4093ed457a6e4656fcf112047b6b9ac97ce0d36e8b98bfae3acc7061b69c10feb5f7dab035ddb35a63a4afe519994e70443dca580b54795bb04408a7f" },
                { "fa", "b6340cfb62c1039ec2f8b7e20a27b05f0a7a547620adc0a829e1133c9663af1c184eb0383172fe676cd7a5ff9b65a9ea8d5c9af3e7b7cdebefdc99efab254cf6" },
                { "ff", "921b0bf3bcd4e61b708f3746bf51ee8df1a72e9e22e8d35ac75bc925ac8378946b8d00b34a28b4250747429cb895f66c47a87c6da9aa90a13cc81134e2bdd693" },
                { "fi", "f08b63f446a85a2ba021ac59c6a9b4d4f8008b8cb4483c4759200bc7da85a9ad44eeb5de91b860340ff114e37a5ccd544864db738bacb30e1352ce5ad5c1bff3" },
                { "fr", "94acb509b5a2991bd32a8e5d156463f21f1787580d14699ebbff4047f6adb2c3497670d3af30f7f477e766ce8d50a6c82101d301bc2fb177eb44d6a7b6b9851f" },
                { "fur", "ce4a65082c67e901d914e050bbdeb42b308b9d808431f979072110a6f5022063de1eca26468b816532ecc8b3461ac0da80d85684d1c4a5945a026981b57cef8a" },
                { "fy-NL", "8f518e06f1578f71111cd4d3490372de6fe6aafb0fbacc0eae24f4398347e1cad1fed3e932235178e2db5d61535c681a35594c142f530a95ad3af97755004a89" },
                { "ga-IE", "d32d600c8e881644820950ad7f45dafc9850ca19d869dd8af4eae706d8b60e83eafadb110f85292ccb9c26bec38b0dc2387f350f1a5fa7b8336fb51f32b4e8d5" },
                { "gd", "e9ea54ab0b26233ce11713322bdf0c183d6d5199b502c5ff165f495d12b9ece5ce5a9e85994d71ec590f75f3839d5a0702d9e90159fa785e2962b7297acd67a3" },
                { "gl", "b4840d54790b9c22e37a1acef1010f3172bba659b9adbd6bd63326ca751cc77318533360c2467aff4cdcb243aa77b8f5431b579025dfd8657bb2f7747cc12305" },
                { "gn", "b1f0a76637c1b22f8f6bea38336d6ea8237b6c0ac0556f5fbba69ec8072df6130ac557b01886864d6bd8819ed19951fe37c2174786acb8cc477e017bfd0fb29e" },
                { "gu-IN", "42cfad51c89c5ca348f2b0ca65ad6fd5a194328ddbe5c19a25968bd2819537081ed028a9f62a86c32146b9fc8d08a09af518de4980af336f58e412c82d19aee9" },
                { "he", "74443109e3a465510d8a0835d903350fe1fe57a6e689570d2e195c220cc5914326f8e0d29404eae3c50daee0615369635c7c26c07f5e1e46afb0c29f740a22ed" },
                { "hi-IN", "a3d0300faab1cabbdaa9fd8db9e347b299b414fcea2f1cda5ee498ba58bd35100c993503a4a9a8cd7d8001a71fd192117e960f4804efe7ef1d277d1166f99abb" },
                { "hr", "cfed7bcb777aedec67d64b733e95e0ff07a9c61d55851f39b95a61d9be5b52006eb822ea03276ea417802a046f3d567d5e0b50d857479de045ac79474920ae1e" },
                { "hsb", "af8daf2272ca481f5bd49750c83f886ab1292ff9b99e99b033ffd323cc0ea696509788ddcf73a7267606f2638bf6ba92d2d742d3961a33424d45fd572f1fdf24" },
                { "hu", "6dc95bee9b36f4ec66af64db8d9c31bfcbf9fb2abac2265c7cc0fa89b07f5223281a63ccd85df94b05dcd6abbc233f6c6525565226e38e8ae0fc6b4ecf26b2c9" },
                { "hy-AM", "5f6cf0d3e5cc00945ad8ae38076bcbd284d8b8785dd8c04827333feca33679e4c1fcbc36e7d62f34b4b032a1b4b5e02dca05a747390cedb55e133e55a47ff161" },
                { "ia", "f386dde21d4417ad2ee56f472c2c7eff363af9ffdf5b08b1ec047a8725ce0435bcd7d91374124f939b33e2717ded86e51e4878a3c0508e862b8bdd9808fc19c7" },
                { "id", "76372fc8fc30f2e33af395cc1b46828984ff85258ce16b7039ab1b8c8d182e9f52650c5e83cf8ffab8b4c90468a217a0126787191dcf2d8a5dfb12037cadc5a5" },
                { "is", "dbecb0b1b3175daf4c46c6ae0cb73afba45be2fde08b0428b40145a6402d2f2fb9da5b52e21272107cd1cd1cc42633ba137ec4597cc5e46dea2a3580778da156" },
                { "it", "04ce20108adf92b9b50e71413ae5612e827272664ab663d06ddde100489713ebb0c8ff128679fe81d8d22b263e0a6252dbad00058d2d9ac766c988083eed0d20" },
                { "ja", "3d26d42557e82dd3c73c7f53e917ae98cfebe93fa4f3f9c28e2350ca3329215cd40b187e1b996a4b4aef7eb4e1fd32bc10eaba20342da40cdc15565582de7efe" },
                { "ka", "ac8eb8eeec95fa80e133004d1df8f556eeb150110533e654919454b45d1bcdf791be03f4ce093bcf8211144fc4a0c28e4bdce0c0eee76ba03109376ead196f9d" },
                { "kab", "20e947a60c97e155be5da20550081290dc729bd331a92672ea2c86d552c7c50415211670aad0df737f3f04c176782fe3bc7058939a1789e618a613022561dc63" },
                { "kk", "4694143ca872fe7026bc915e0b1441b81af1bec04f8102f98e20955920e8d2965f421fa3480ccecd59a537de7eb7c44bc76b9580e5ae1599584a52dc76b6e6ed" },
                { "km", "148d4162823e5ec11afe079a93051a2d1e3275adb96ae927df38aba0aa52abc857b45526cf7027312878a6a5fa5fed6a55edb278ad9a82a2b8432aa7ebe8fed0" },
                { "kn", "9dc609e02b794dcf840ab402e3a287c1ba1a30ca50bfe10835079ccf1d7338a6551a6651dd5797e4ab603b96e086c63074770f9bca6b39b0287309f34492b42b" },
                { "ko", "cfa4ed1f5e74e50550206fbda72085ea2b0437a67d99a61a19bd029165e719504c7fbc169eb5d7d3b3167c6a03f3045f9bff0518dec655054a67c62b3820427c" },
                { "lij", "ae7aebfbbaadfcc0ddaa47c2604762dd9887c7c91a11a9a3a3651d63bfad12ae1998353d1f072cc741b2fcd0fe852113996752d17fe9943ae4e072384d4a5655" },
                { "lt", "5c1edaf2f8b4bdd43ab92eeed3a40e469f454bd9f542cb0d598d11504ce4067131e6c328821adcc9e2a1b43c56a3d447efd41d67eb56eec115625e88e7e38ac3" },
                { "lv", "9eea8c514acd9656f2e72ce66c809f8e647e35961bdf9fe15e61b98c9ec15e0d22ed8a4edfdc5966cc13f943fc3f3f45a1c25e48969ce51104176866e38b1660" },
                { "mk", "49607cff176e374d862a12f461dacd03962cd40908b9b072e24d1ee8dba036ba7571a59fbcd43548be558a9590ba69d4daecfb46dec6c57d20d7a84a4a5b4231" },
                { "mr", "6173ac1dc44bce362218e2a0dfbb9bac21576f694ed88f131380c59aab6cebfb1b9497b228ad60f69a46ca1ef352d238161646c5e7a79381d1a077d828a41588" },
                { "ms", "2c5861cfe21d0ee9902062d6c19888bae3d53aa8bd4fd1f63c4c3208ed3e4a1c58c93b944093d2991de0c42eeec26017ac6d3701fac9b927e0639220aec4b4a9" },
                { "my", "8e440941e410a9c37a6c822e7cb2f3e0972a6f5779c8e66afdb552665d93a0a70e8cacef9ff6d5a83414999a5149be902806bc37804a81b53ed36bef7706145a" },
                { "nb-NO", "8d11967676d0280935e5ce409cc2cd099218df57880370dac3db7543a16cb21ab07ad4a6ad3ae4fb55f0ae59dd1830932a2bcb10cde9d15afd05f7bcd8be5f14" },
                { "ne-NP", "7651ecd58544e40da516537d702c4e613b1672979b323c0422dcc8d465e62d4ed57662ca295cad2cfa89dbf40bb3166161bcf62f7209f8208633fa305dc41594" },
                { "nl", "1db885869de3bb53780b714c8dc47a1c3dfbd772f33ff3f85f79792ac690684c59058777550b7df2ddcb428ecdb519587a9c44af6d8fd01d503353f980e5fe63" },
                { "nn-NO", "cd926ecec7dce41d7c5a296f764988b436f55699b67b525712dcd4c80a644f04b68b7c4e883f8ec1436b24f7c870858216ba08c3fcce50527305fd3c2c2007e3" },
                { "oc", "f3dfea56dec2e3c9831051731e143265393e534fb863331aa3f1f8b285362355bc95149c3b4e80a1c9b1e3152b389f0f0ba00ab9e279e6893aa14bde418b0e2d" },
                { "pa-IN", "b6fb87c49ee68452f8014e0a70885d0e523613168eb079b5b70d403245ebe1cc6ac044f1e240581d5b48429521cd3251821a6a8559e0d11401a06fde856fad98" },
                { "pl", "f173a4095f11192ca7db38793f51e04e9e65c2895bb5241eb0e45b1b36e8603afbc0a71d32be0ce717baaa03ba41f7750dcdfae1cec7ecb6a9d663b860519a30" },
                { "pt-BR", "8fb239cdfaf4232e0d34ad3f48fc7cb7110d784d997121efd4bd0e76faa3140b5f0f48014b1ae5390e0e1f25f17a343cc114aac35b27594b847b9d02b0b305aa" },
                { "pt-PT", "78fd6a31b19be63a8a31969229bb0f28ba17af2fbded75b9772d273d1a55de18c6a54734bddca76594a3ede72c8d64ac2eaf2095221123eba4c2f45e610780f1" },
                { "rm", "aeebc0d09edef01995374eb34bb147da5568f08b7809c5029322f4d1b1d6567ef7a8e44250c371eb5379941a5285110c79c22daf7e03fa84a6364a5aa85fca3b" },
                { "ro", "e492ebb1b331f60b9d2baf1e97c2b429d6ce061ae7569e5655e31cd638b730ce5155ab3067f4cffaa6f23a920d4bcf309631843ca9ded6d4b74c322c3f6a0afd" },
                { "ru", "14450c75702bc6098523d8c069dc47b81993193eed0fff1768a57ba4355655cdb3e0952a36f3cbd1f295ca936ba90d7af526c5402af903ea898899ddbcc90bce" },
                { "sat", "9bc53510f43c2548c360d533fae0f7830acc204874581c2b1e3f4ac1567f3c5a03feecef90c3530d8086adbbf0cfc6d26c9eed2feede231a1743861c410a6d71" },
                { "sc", "e023534ff8f0cca8d017191a9d93920ce8d05e633317a98848fa3fd5064670954d2144f8e73e21d67cec8289fe33f37f6812e999a0bbe080f1765d5c44ff362e" },
                { "sco", "338e56268948cd2fec480156d428beb7a5ec2535dd0491e742b9444a276b4877e204b05eeb07bb36a45930c396357a7975b531426706c276950b5b0062fca327" },
                { "si", "edca85177812e03ba6a991e66c9ce8f7c5676d1f032037000e69369a611ba518b7910748a767c1654daf49fcbf4083303a87c86c5401fab649a070192d4d4f8d" },
                { "sk", "893a624c1660706ee9243cda44348f08c1090cb8cb99e56389ca6b6f016037e93ecb48fce5145ecd05082bade92e2c04215570e5f9ba18d4c0b8ca9311484ce9" },
                { "skr", "1c244a4ceb09b5c75b6fe4535f96e1fadaa8f32673d8407f089438c82c3e5eab959208018200eccf54f41a570772bc9eb96543d845192d3b2b0b3b05b473ea41" },
                { "sl", "714c0d5cd9b8059c55f474ac41ff5070c4466673f8399a51c6264770ebfcea8718f6fcb7df02d25e212b54f50f064ef6c3b5657cdc813d08abd8e4b7f1dd67ab" },
                { "son", "79ade9c20763f50176bfa9b63898bf4bb35505a864ea2daeeb9aa1bd087d1065a60ec2fe4f52bae4700877664adb9eeed03f88cd9aca51a3c4f0422071aa091d" },
                { "sq", "1f66c9d9513b83e1f28c273c0f7092dc8fcb35d882dba6a1431e08015fec28edc26b4acd6ebbcdfab70207f3b566b3823b86a53fe25c67363eaa000e3c87b9da" },
                { "sr", "dbdd29df87db2ad93fbacf8f08e0862c1b9e2d9199af8c9dc3e144bfc487fb5891d95b775a95b03c94e9453d739dfcb7ac9b75af239293657115aaf9361e0d96" },
                { "sv-SE", "1ed9e423f07fe7e442c4327f81782025042a78f472dad91d6251df9f7291018e7d2f588dfa44b4a531901ba70aacc9ffef936865e4f1963212d5ee8886a34a27" },
                { "szl", "176812de57e5dbbe7ce30f8168fa0af33467bb76f455000794081ed2185cb727e09578596297a19f746412e8365c6c895d5395736f054afbea6a606897c8f602" },
                { "ta", "c090f47c3728474482ad56b75010814642250d1f0815eaf5bd976328012556285d1c7af5126e7c761c48eb88c15db33d57ef1d0be9bd1c3d50fd6ff8b68442d8" },
                { "te", "24a2508ceac7e29828018b8fabe2289d20776b2f24fa326803ac0c89220d20984295fceeb2618ae4411b8f3731165db785b43c99596e5abea5e050830c2233c8" },
                { "tg", "326585c0357046d892b3778668357933b1af8700e2fe15316e2487250fbbf192c08604448992868d921c3195fb3b0ff2dfe2aacdc59dea5e260a962d637ef83a" },
                { "th", "c2be7bda79fa981cb1d3eef28666637bacf39fa90038a82517292a3d3e6ab85a138aebef69a5031b4f94a2202d067abad910280567ae0c1520f900425cc1fc5e" },
                { "tl", "4223a2cccf46c62c9aeace5b477bcdd4ff49f94bede8c2be7fa7e15e3b8a24445f5b47aa38b1a04ad7eae963f7725778a60a470c20de82b797648370c7eb379d" },
                { "tr", "9a12fc92cf8174682807a5fa853065288859f10e378de390c1d46c31bfc322a69b052da8b3febab1c9bc78a1bba29ab294204ee007a2055b6a5bfa26cd8e8e1d" },
                { "trs", "3283ebfd64ec22bb013e7e97997cf2a53044e46bac8be0dd59f7d574cf64249d3336d17467291b78b2f8906efb7231e9cb3f6871dd0efe011c095d5c8816053c" },
                { "uk", "dbfaa75f666fc064e3e755e14c692dca37c90ebb02a749bf39818eb3624c56fbb6855e115985887f7e8eadbf42efdc44142537ba76b75489d3800baa6e214c17" },
                { "ur", "6ed22b7b15cb856d5e840df2c3389aac94c51f97b0d3c1cb228533d58377cd98b1fc4544357d4c171b1f6e033ba095d3f2f40aad516ec8bbd75f2ac9982eccbd" },
                { "uz", "3ae2356d0d83e0e77c5bba2a1b409fb242f9c01b455dd6a8e93edc5de2bafce277066ce7261e27770e2849421bc6f1928b37258f3ab168e1bf24a117a0f3ae6a" },
                { "vi", "f4556603a193faee458f890879e619ab0bad98421b4618a88ea2d0e68d9ce984b314a02c4fefcf617f282e174798cfbcd0084610ccd2201d9fb360f51125c0eb" },
                { "xh", "5e856dad6cb35692cb9250998671dcbaf6eab319620d065860e994a9e6538a4a8b3abecc9d0a6236da4d45ec0068983eaa99c9b41f8c18d445dda672fc8896ce" },
                { "zh-CN", "c77a01598afd7d6887d994af535b7881ef0e76a41063cd1664ebc0943b4d7fdba2422fd313d0c5fb28b064d9fa439b9ca26e33966a37169871e38ff9b6009cce" },
                { "zh-TW", "a5c261e4aebdccc6729958f473fa0dc83c50943b984306bd13a0330818dfefae1a4d096c3ce58d46382d7537f3f68c177c6529055a3ac576ac20b914a300146d" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64-bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/137.0b9/SHA512SUMS
            return new Dictionary<string, string>(102)
            {
                { "ach", "fcbd4639e81808e4123223e47d37613b59aca26053a1c21ea691a47e4c849555462c3e036bd521192ad47a78ebde4f19028face90fc66fd971f672f008434cb6" },
                { "af", "40412e71b98097e9d6754c7358a758f21432f020e247c20722c3ee6a80fcc62a764bd467f1511bfecc7c9ea8b05d7f7a0d9d24a1eaf3ebc24bd1377bb56dc784" },
                { "an", "4d2310d40c6bd0a66d44e72f8bbbeebde270ae6e4291671db148f03746e96dc6a5cfcd5b265f97f37f5c0e11c08afa1b41c53fafed95108cb3488bf377ccfa1f" },
                { "ar", "8bb608f47f10190fc2e3c4d0e33782636d73bfa1851a3e013c5c4c470d02b303fca1b3e661ae70c5765b54ade79a3522e6be5f20b05b69aa9b6b996d68a52b43" },
                { "ast", "2fc94697661185f8a20255200ed5828cd51f100497ed4cec9907405c10e6f899df8d93779cc08eb81b5e7d1db644cc903767549bbc0fb65ab5bb309d5eebf61f" },
                { "az", "3c919562a20b7562dceb043483d8a3199061457354e1f962e3f70b15ac1b9845ed7740a794a6ab497299355bb7963b3223f8ffa9ad735b5f140cb173e97a8086" },
                { "be", "b04ff38fec2411634dda325840b09a0b0016a5cf329573eb38fff27ca9c2b1a425314180adb42befb3b4200b0c2962643fb10ae9a251ad7198eb0dc8d82028d7" },
                { "bg", "e293e74307ade93d7ce5939907c9975fdab38c3e5cca78a440c24a6d2144d65de2cc0243092d270fc09eddf12cae4c117e8090d3682a52f30e7bc466f0869ca9" },
                { "bn", "2631315868e5aee5ad1244586b1b65a89bda22d2b4d71b6d67e7ee3e731a2f8a18e430bc6f221727aa6535590ef480c48847a773c2a4ea7aa93aeec91b4f098d" },
                { "br", "1bab321ca3468659ac2d0c3feed2eb0d6ddc614ecc2f49aa758f7be3c39e38c775b04c537aee546617ec881f4ceb291aa5d94804d6fe8494f676998dc1eed3fa" },
                { "bs", "7ab1d1d5c6b4ad375f630ea6a7a81ef376708513cd7964ed08545d9b9ca38c555be7b1b4c67a6c89fc1f60bb17c66e4e611413e70491a3ff5771bc5e2b418fb1" },
                { "ca", "02579d06b6fad1f848fab3d0eccc86805864a50cf5730768704bc9d8c58e6fe66e863b8eaf69d2dc28bfe0bf89863fe14cabe84c04d3702a4443a415227e44b6" },
                { "cak", "84f5c1e65e853ea5e7963db963e8531f9920d3a5e3fe3f3096be460a34f30f4353d084874b3cb0df97e3e280369cbcb333bfcf82687837309b0c6501abf10a70" },
                { "cs", "535b6a2dbd60bd014222c1456c9a950b7a7bb79908d47487f56342007e456693201905473b5f3703c796d21bea67454047c4db9a578857051ee5d031a3e8975b" },
                { "cy", "1e44510f61e608b34cb2dca35d19535f6d21cd2666d6b053b898b5226c3f46eccf2d1534439aa2fd5e7073e0698c04c1aa372dad90e61964f06c96f83888d90c" },
                { "da", "0e6cb3b2bb0943ace2326446222621d8d3cbbfc221d887fdab754fa0e261b5a3d8e525dbf105ef3df09fb44a7e81dda408945a743711bacc64dafcb5000ae091" },
                { "de", "4f9dbf81d58b7331e317ec85aa37d635a2edcf695cbc1fcef42b86880079f4cb4d76f01cd643d8f30b85bc005eb5725959a0083acc651026c4202f1e322926a9" },
                { "dsb", "0cdb0ea982ae73d15faef22338e67996b779853b375d22235e3ae903aa98dcdcfde5af4b50f1d05a7483bc798ea1778b7b7df80adbed0cc5376b45dd8394bcfc" },
                { "el", "e8e383825c759380ee961d4d7e531fd3ea981c533c044513f4299cc9ec7d0239284bea338562c738a8c1bcfb57b73170e68daf2f1b3bfae354b8b7a29ed1d7be" },
                { "en-CA", "642355643b329e80c26cebbfde1460f04f4a4ac20888981f684064a1705c432479da2d1ae29dd18215880d79d18700a9173da48a7885f4e60a62029b0e8f4e9b" },
                { "en-GB", "6b34f1ea836a55bf922671dcdd1b808d09c8afb82a7e2dd2eb4e6ec634ca997aa10f074b168081000baded3062ee746e342cb312d791c2c82944c2c5542b5dec" },
                { "en-US", "5eb993d3497812872a2c5167d0c05c16e49403b310c9af94e03cfa7eef3b6a579b798b4af6477d0e777609dfa8e93cec6385bc708525d65c3add9f54afed8aac" },
                { "eo", "4edae81929c6c12c039f44a1ea35295623fc28a70bf6bbe43bc80c324617b0233e5ac0de8caf3cb733babbcac0cd1ae9ccc6d2c4a72af9023cc7575c2824fc63" },
                { "es-AR", "5eab579aa23a060bc765c0c8312faa6663f0e5d621f20c6df4752adbb9b615331279fe90241c50b07a0ef98ea4e6b96c14364b0bb5b3f4943539110e49d0a4f2" },
                { "es-CL", "05a13502bb4f91f9e45f28225ae567ab248aaeb79da7381ee8b8cb66b1972ca87bac5559543a24e624ecb68c6a131bc69cbc4b9acdf08d38116476aa3c66849f" },
                { "es-ES", "04c2f2c4ad093c45c71faa851ab760dc48b7527cdb30caf253441dd95960486195532c0007def98cb2a6e49362a8c0bfcf711c5f30434fa8f44895c0fe4772ff" },
                { "es-MX", "92962d94830ddae47d963990897f8067d175d8f20b032c873c639775ca9720b05b9575e3f348e1654a8f6313330a86cb5f32b9a4bcd0c4121411c108166d7896" },
                { "et", "9eee169c845728e691407fbe5850eed163166f28710d59573493598261d01c864decc41e1b3dac0b3e3784b6e2d63a7694d4b5e1c6935a59edc71b5e63390ab7" },
                { "eu", "1208744342da53fda05e3e784a08ffce796692b4c0d208d1d461b9c0cc38f4ea378fd53206f8a702397d7320246422548a2485b6438162b22778dedbbe5ec4ff" },
                { "fa", "9a27e793966586a95138bcb1de51a252c48f876585301dfd8d4fa9a4b095a1b66ee69dc045f6bd23c8ef568bc5aede96b8478ea2610e9d8ae71c90753c534098" },
                { "ff", "53cac4ff69c7106032c0f4effbbbeb44ac0f907e12cda841a434daa23b4c4a63dab49a6431c206b4ee2a9fadde3b4ba1d4e12381b68a60b306e9a1a03e25a210" },
                { "fi", "80f8ffeb12848c3e2ad4f4d9b50ba72d518b9c7cda6b40c0845d15cfee5caab2c9fb613ba6de1e8ebdbf21b2a976d84a2746514b37a99a0d50b21a468445d66b" },
                { "fr", "88161d47c200b9ce047232801ce39978e449e812ae3ad84731233485d82c06fc826aca13997cc1e45c1aed51436e88107cc4f3640972ab811a17a4cf4439b570" },
                { "fur", "da54c0fd48980c188c793c1cfa6922cd79e2c660b91d4b476e8122688544bddacc38e85dad2a5979ec2ea8a2e36ea0d42477302221000c4972aa0c87cf741397" },
                { "fy-NL", "66c2e031e1904fa08c5f85985f554240ed7203307ce2c1031d3da112de6df37f73895d666c83a186df8bc3b6c7fd6cc28ec4e8348b662e9e5a41055028c608f6" },
                { "ga-IE", "99843b493e8f8db09bb2c0f3cbba6cb21ba60e0850494226a996ec103e2120d23adcd89605bb80c3ba061ca2afdf7e5c43c6bad085a54e3680bda8fda3a2cddf" },
                { "gd", "2eef09a6c4ca2107d7fcb6994becdef5cfd313307fd564120b0ddf489c03a34cc0b18eb2ef1ed9e118da9372b09c4276be123ed56a48afabc0b2bd3efd04bede" },
                { "gl", "3acc7eed87835d8fa573fff84c0f34a7502fa8bb6fe50e37d39b9f6d65fb86a4fd772d2dbf0e39f7678c16f187c46e04f4776443d2767a66e6d7db6466a373e8" },
                { "gn", "5764b9a49c625145c2f789a1b625be20e2f761329dfe2d22b471c8de556102468f80faeff4440d1028d3bd3e985eacc32444a6be7bd745a2f8a25c36e4b817df" },
                { "gu-IN", "c74aa780b581036368533b82c6b8e6f8571fa3c84be5e0201471938a9f1a08c3d9831615dd57d7f171fb1de6cb60dc5a3fcbb2a1cbd5259e7384929a6cf7b57e" },
                { "he", "4d5239257dfd526f1ef99f5fde1ea1e79796a524cd8d923092d019ae6bddb024d1adc293fe7bcba3094eadb53148a8e1ebb089d491437a57078b2119d5e8c766" },
                { "hi-IN", "b45fc045bee05af80e90d01c914d03043508d5a93ad1c32edbeb2dda212cefb5e24fd3ee9c7eb2a432ae62cad1fa37b3a68766a1ec0d045a55bf462fd8379496" },
                { "hr", "2f57a95d786801ee98d92335b4c8e299d2c112e914c44a466f9924ce72e5d6a5b5a1be3f0fc4b32d63a237c0d149411292525f7dd3b9793587d5724a662e52e9" },
                { "hsb", "461c2e349bb281bc5cc692ac9ec801cbb7fc98e3d838aa677e31965deeb26a4bd2f580dc828b9a5ca63db142a7647eb261fa96eed0e8fc7351a687896138af11" },
                { "hu", "84fd1ff4908792d380aedf55b512f7be5a071edfe2b6a04bf9fdb46148b48dfe7173fd3ba453178833d5656b826ea1e5a3b853d8b936eed31d2827d60e4476ec" },
                { "hy-AM", "47c5d9cb5f553e13164c7603a12fb24a1126eb11a96691976ea51b195cbd9f7a08eaf916b1e8dace8a57a6cd36c2e4e3808f9d6661f98d70dec27fdcab3beba5" },
                { "ia", "da65c956079c5613f4e1f17d84850b97dcc3bcb2567f5a07b758330362f632dd1dc58f726e6c6442e886771c5a8d59dedaf21eba8b7cca6689148f6f351226c2" },
                { "id", "530e6a8d9394f58c2ae1de75e58d2d3828cf7ba0ca67e2fa88b83b9ae4a1b5e8cbb2befa5c8b9b471bd56ae8fe38781afba3b3d341374047b459347e088ef1b8" },
                { "is", "60a41af7e2e4628d90d8e7ff5ae4cce949a7eb578670d4b2eb7b68be46e261368ccaf8a3d99a805a7e129797b60bfd8225bb0d7023b8b8c89fbe07bd4db3e461" },
                { "it", "f4dc704f378dc48f2f57570f1249a384133cb6358c3b9d2e609a142fc182cf6914cdf33a96a784affbce470f9ca86110bedde9ee1788e3771871378f2d9ffd03" },
                { "ja", "7ae12f5439181bb98fa2cacf0eef2c98b8e4992e7a6b703a280554634b820d167c10689ea55828b0085bd9e98ed193fc3479988d35001746ce3f7fe09b5283a4" },
                { "ka", "eb58ad5275831942b6cb016569fb5ea0b01878dfc6b4d18a946e7940488b4e00a730856a332309271927718af117254d0e2099675e296e1c88b8dbea57840aa4" },
                { "kab", "9e11cacc46b6db512d2ee56413e2a778e6134b9a46a6abdf79f2ab61ee306c509d2f0fd22984b292f319128cd4a6071b394264c0a9aca399f0aa362fbc587592" },
                { "kk", "9d30f2e143c6c2e58c521c6c74e7a95a59e131a39a28af6f84d8e1255bad187d661978918a30e59c73933fc5eee70700be9a0660488cb4d20c549ee4828d0aa4" },
                { "km", "dec950c8a4f4086006eba532a30de0a868fccbe920ef2f8cc0565c5e2468ffe96eec2d1a5f2941fa4adca490d4c5e7b9ae8d4e9be932b465ceb86a0be5dd236d" },
                { "kn", "c801e69f05ba987249d74b2681f0f8f3477143175ac6bd9bd4afcab1367abaf146727d3046c509437778f95801f3d86daeecdb76e90fe6d5c1bbe4749378fab4" },
                { "ko", "647c61a9c162a8f67112069be64d500ccbd206e0ffd536f2eb619e4d8a213ec23ebc50dfc65285ff31fd2c86d760463a855c991a620d5a07547bb433ad5e0e15" },
                { "lij", "0588963b55b199d29cd069bd8d4a139f8c91b584d1b40d842dabf0945c67676a5b5f102cc18da9fadf12acfc3a0d3fbf0b2a164eebc6f09ba5679c00e87cda68" },
                { "lt", "57eba32c7b24c9ac06ab5e8d1ae8bc9a69a329a7baa21bdc85c5fb2a94e6328d3368f0916e543f060432eb42ceb031bf8e304529b2bc5089b6d61d15a7af7ba4" },
                { "lv", "67ddc4f6897ee2b22f53c4987e38455b551dc36502f0ae3ddea0be6c31475e43dcacbec5b00d850e2b3f9fc426feddc4aa61f9db4941ea7f35872cbecbeb8dcc" },
                { "mk", "9fefc98e419bea7f90346163cec041506cf68c751fba5e2f672aaa8def265e2b5b73f73ab590c35fea7a9bbe57c65abf5172404ccf66cf5f120496350dd08f94" },
                { "mr", "78e64c93f8b0e675b12c1e849face6fdb7c6190534a51eff9b5de0121b204abb451b4f7f1bc3d57535face00a0a7c0392f9d52fe0329c185569dbaf397d4f85d" },
                { "ms", "9e5833d3af26d1c15e22bde2830242f14c84e07c89360f09fe5b1244cefb3b89b2643541f27a80558302e382578e9da0ad3911b74c346d9b3976502ca3ab1b10" },
                { "my", "09c45897bb312ab74f8f6a1ca5667a221c1ea7b8d8d9e08a87dd90aac6b854adcc2630b3937cd6f2f1b478231105cbe5ed755d2775a99f999b631c88993f21aa" },
                { "nb-NO", "83d37ab05b185da5e5519a4c50a226366ae0d78f96a4f17f7df7e343c37754e3bd659fb68bcb9ed74f4c57592f2996b538160898883aa862542581162e255ac5" },
                { "ne-NP", "c293c85f89479af50432af6bdfe2392fe107889e886a941f6b297fedb79f0ba633cf32021cd01fdce4448a8762209c50d280b59b41ef69d0c734c193dcccd285" },
                { "nl", "82b6544724d6ee4ea2e95947037f522cdde158b37b60fd60d573a51c9dec9c554c9c646a8fe02a32a197bb388805d5ded46e9d2c77cc27b7fd07b944ac9b3268" },
                { "nn-NO", "b9f72c42851a4b8ebc4b863192ea82f4f34d65e6c30601b7e8e360ecc34ca22fb1e7874168af27e3e736b94e76ac8006151ce93f624408aa302dff84b864394f" },
                { "oc", "39ce1ffc8f16b395dcb500b7eaaeb91c8c0111d2f9fb1f26b738e45f43895ef2adf8b88f9219d6997cbc715061ad288b65d04876f2f2b2c334ee324773d58c7c" },
                { "pa-IN", "eaf89c0274fb005fc5b6f6963ebfc1049ec0f7d5ef92302ab8b4bd6c5af93b52ed99ab6bd26de79466916a6ea97c5522faad1d77c2f63f7d0a94530a6fe81971" },
                { "pl", "783a9baa7b463be11c500b0f46509c9ef08fd28209fcaac059202682cf9d70c3b3a02ac66329a5de3079532dd4213e2cc75eacb195d5b4aff99fb7ded7025ade" },
                { "pt-BR", "8ad40194f5f53de1cc9fd9daec744fe54a2419c190adb43c63692041770eb88624217ec120517532274d526a11766b4f9a2e15c4fe223e8ee06c411810c0d4c9" },
                { "pt-PT", "4c32a422311319dd682c3079f5a7091ed6fa3f008746826b7f901e512c260f4cdf325ab7d553b3fe830f542a3d7d5b7a5802bd8c7c6e448847044593177b109c" },
                { "rm", "e8e550579a72de8bd9d865019051b1ffc00c4b8ce1862bcdba00c27fceea5e16171e41221ff7d5f1d3c3de0d3cd48823099b35a171f8e99fa2129bf695affa14" },
                { "ro", "c7b2896bbd18d5a20c93fd7184491cd6db27a4957a334b01f9ddf2482a1960805f19a38a90ffa40b79372c994054fd0fe6b1ce6fd6930ee92a07de08023b0747" },
                { "ru", "e1857bd9210a96796cc8c4d96ec5f2ad4b8f3d1ba4f91a8bad350d807dac2d463b3eab2fea0e7ed42b25d4b841518e3049e98bcf30d605c38f2fbabdaba69158" },
                { "sat", "b6df90648aa6dd3245ee8176b5620f1804b3fb49c921df41d0d9d48eea145650fb4983a97e325879b9024aa2d9513c0459de16d2385b007aefff94209696f9d1" },
                { "sc", "d9f92de806f4c12d11ca4c4b57273addf60eccc77caf7a8130ada0a00dd6687f3bf34f5fc17554a61379e2dd53060be089c14cbe15ee20b4219ebd616b26f77d" },
                { "sco", "76f033b667736006dfed60c85b27ad9c613b6f9dd7bb576bcc121d75640e5e3210dbaf66e7f6e03738542c8c7dfdfb440921608192cb9b0c75efd0d6628479f9" },
                { "si", "92b9753029af085587a99662c3fe71957e46356fb737590be600f116f1f12caa09fbf4727fd102a2277eeb2df81cd765ff9cff3b7eec96249a869a742ab14b4e" },
                { "sk", "6235aaec19f2fead86839c0c1c0136e512976388df152c76914ab4cbc1bf377fa0bc261f437fe30534666c6e41cb34bd2e8008e98e5457b64374719d0e10d752" },
                { "skr", "b5ce21e8f458cb1462acabe5e3058e88278623a1cc14e55d62d689e6c01a18018d3d143bd99ee4751a304edcc0e0624c5d2a82b22a5ab6020e785afd5ac7d2e1" },
                { "sl", "e0777153266f23b50b5b1835771d034ed1fc5b6628245108574d8fbde314332bd49e12f9edb82e6d3d560cd9b00a33b4bc23a12b62f442d00806a9cd01d060ea" },
                { "son", "5221e9fc798918d31fa4d35204b7a265ea002ed04a7eed39b472be2bada394790f5fb6474c390c44f0e6f4b29e01244ff2a51e6f7909a23d4a58363c9cfe3d4b" },
                { "sq", "c860a3aa14408a01bc75fbca9fc9cab9d8ad8b4913268567219afa1bdf7c11ca751b4f5e6ed5b496175ac9c31c6c2a78fadc1c26678408c0bdef849e67036e4d" },
                { "sr", "459e6505dfba6857f81cd0d13f1e951e6505fd28a2bb410892add8ea33f48c867b9df9658fc906da02a725dfc23d67aaee0d85eb645000b333fe9ff169aa14af" },
                { "sv-SE", "14910c36692715c54a393d004e2b3c505c35882fbd921605e4d9e96f97eb552032379e9ac638948b72d09df1c88718af3d2cafe3d5df0db83294a04b6072f416" },
                { "szl", "d506cbb0c249226bfc205b80df2fd7b44dd3651fb009784741be301ce54aaebce1766ec26b9b03e576691c939c1c05d0bb127d0f24babc6be4b537d58986af04" },
                { "ta", "31f8ef089daa89206958023f2ca1ef2f7ef2aa0ed2f928cdd2a943e245d689dfc61f554b69f6d4055561a5f37e45eb80511fb1cbfba5d6e679571278f1609b60" },
                { "te", "9f419afc0fb14a26b5954d1c94ccdfa0a274df2a221623f63eb816d5a985ef6dbe639be8fbc2f4389896aaf6b96ec027c60131965fd5593e38d107646fb4c480" },
                { "tg", "ac129aa23dfc2f6620946da24ffe11799619784110d0a5c7c389ed8d9b21b7d22497529ca0745aa3b396233cab14bec2b61a32c9160fd5a7e178b3b89dd25e89" },
                { "th", "04c0b6aa3a40d7a96845c37995a61c1b6ae796b60be68814300e019d28c0bb6eeb26de5e3de70e33555fb3f6a4d7f739e6a5ecdec793fc4d4fc8a74bb448a200" },
                { "tl", "59fa444494c87c79e8fa8be3523ddef67c9e3b2c3af0ea9a83578d191f4fee2acd0a54a4c2887f702718af32f8fb1def2cf21d002d3d65e79cceaa56e6d0920b" },
                { "tr", "57c9f28df6b375a3c4cc6be452f9ad461126d0210e3383fb6bdb603ef5aeaac301d2a06d6435abc66d193171af7e8b43cd5d4c9375ff369b53b2d79f0077828e" },
                { "trs", "e6d62beba6cea2e9783b25be2f8585bdd0d141c9f948eeeed4d801eebcae8148bdfeb27cc057a9dbfe30a1e32a0f121a38e84d23de38ff07f3baf951e64305e8" },
                { "uk", "c9e96998009c9fb2f01ed89cfc453679c653f40be16d1e9c873a41152b639a44a8f0146c19e7f4b83747dad0c16efb3dd866adb20c2a8213022aa359b78a0831" },
                { "ur", "b6a139a39246d334f2618a291391210bbf63fc1ac01f266bb2895b3dd32faaa1c1737775b79dc79ee55ae6edc73bfc2577e7f1350826d3587885a9eef16ed45f" },
                { "uz", "4df58ab78d434441c3ca206fe14e157d29b7471bcb1245b50aa43d2f8656c97db801e21884c9320f8c36f14f64fd1feeb54a27fd3b2d58b0b0a03641dee98961" },
                { "vi", "5b45bbf6189e78781f76ddd90b56aa91894f8b1d06a8b8e81bb8e7b7bbaedd05bb11025eb37838779ea9d40a0f022339ebb21df94e59b297028b93f31af17d14" },
                { "xh", "b29b6ff2599566a15ead0b9384a6a5ca71e66b08dfb5c403e1cf7bc048ac02d95da92b38d28a3796b5a663aef5b390fb324111a965301eee40943cdfad845674" },
                { "zh-CN", "bc1e2991df453b664971fa9157a870f93411b2034d7ba91ae3d14ac6ca31199e320499ca3d889ea50c7cdc7ec3d269c14f56a4d88672d13ada7caee36fb2ea23" },
                { "zh-TW", "0b024f01eeab74e568e4c8aef4b36203d144c2b79ab64576941310c8c716a070cd620e46e101e4fdef665144c775d188312ec86f5a9a17ea84f69bc17c777bf8" }
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
