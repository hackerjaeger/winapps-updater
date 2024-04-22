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
        /// publisher name for signed executables of Firefox Aurora
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "126.0b4";

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
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "0f4b44a74b07fd54366f3da0cfaaa20302a6b3e8e266d89ada93d380dae558d2a88ecb1a09138f7530ac75e7d027ec7e6a3c0c0263c0872bfffec325da98b65b" },
                { "af", "b05211db8ff42f946537eb505036983ab48c062cd202458342034c89e5696deee6b5c0ae4de3271cc2774a2433f08013d3e1d9815ac1558db2824c716f7cb7da" },
                { "an", "1972c5bb141bb21ce6223eb6e45817f997f11a9528b1a611f3853b20c7374067e8878a8212917a441168248d6ab4ea3be6399ad7b57028ee7d7fddd38d11ca0c" },
                { "ar", "9dacc24553783067545c879b08e21b2d7f7a2dbaa7f2443e79638bbcee469eaca96475cce9273394cf33c1ef4c696bf0a4c43995f1f4c722a047a50302037282" },
                { "ast", "af58968c6f7c25f1fdb3a69b6ac5409c5eb32140ea785e163dc0205892ac4faccf7c92a1b2b4d55130cf0107f8d35c2cee4ac2f114353664e99b07f2f45b3765" },
                { "az", "8abfd0fca3e43bb54cde0588aa15480f0099ec10e746c0e4c29fcaa2d5d5c133f8c3878fc76318935ff3afc5d8a27e45f3b1317c9acb31d01860462ac9fa0bc8" },
                { "be", "d2abd6c243b5b5c34149413e9f3b845f0fdee914e50dbeec087a72df2c5dbf3dfbe0abbbaab1e5c72ae1d54c79ee5eedd311c125c4a0271dfc3fb919c02ffb17" },
                { "bg", "cf1d46cb50d79640d794f9b8f797f23f9c8356b44cf6ec66b81d2f91e765d5089945ab4e148b50c1bd041e1339bb4cfcbea0bfd55fd04b1869f769ecab059362" },
                { "bn", "1f2ee5a428c144546bb5e75356ddfd320a7b75269e7535d1afc990e985b5aecd8476398b73f2bdd203060208225eaa304f2fddf567c5269892f2a5f4048f8238" },
                { "br", "76cc4b53e829c3697d80a91ce89cd4ec23fa9a22af740ac561b4711023ac3165974efe5ad646f4c5103e635fb8e7d75917d09a4401a7a1f8a783b78ff36aa3e6" },
                { "bs", "582137c7038a5a2f9800340cf13ed2b0232d1702dd90de32c0c9942e04eeb827fb26b8fc84043b101eb301b2cefc40597a7bb7ae05d248949e5414d56fe3b3b2" },
                { "ca", "0252096e76ddc2665701ee995137270fc63d5e7471ca79091ed9f344e60ef367743d1260a5f47f637aba016a815e9fb1ae67d8c7e6a7a6d87bc8552d9fbfb4fa" },
                { "cak", "5e73793cafcb9e6b3ede4eabc8a30cff53e203a78d8fccc4057d061ec980d6a2d7aa9421e6ad3d884855cbb94b1b22e2286457237591a27d59cbcf53e1db96b9" },
                { "cs", "486b618cbf561dc5cd4e2ba3f8978cfb32492d00b0fce132171e282f138af33eab2d23500c454f32bb66106b19fefa4e417e0699d52ca4a4cc8c3ebbd986a6c4" },
                { "cy", "730d26ea2b0db26595f1955285185afc84962c11190790c2cdb610958f442c2e5a665617e93a2b63435be1cbdf07e51c4e006660ba401bbfcdfc64a8b0ec5456" },
                { "da", "87ae8fa5816d277b3a40c738feb3655dedc72f16741220154ff0a78182c634a1940a0face90007c7db08b880ac5517b5e92438f4f9dc3499b89ad06fa6a35dcc" },
                { "de", "433762e8dccb25a5bb92367569fe48364ef0903b08ebb4cf5cf264e68368f704fdacd0fa499494d02841e3b47f4f3e80d1dc40cf56a7ad39d3e746c9129fb418" },
                { "dsb", "8fb1386356a85b0cefceba3d7c5f27db6925e302aa197779899a5ecadd2b88b2b1a7ecce4e5ace5b5082c9b605c241f9c2297ecadc43a81597b5b90032d20b54" },
                { "el", "91797bdffc23c2235a931df6e25afce8fe542cf2548735ce494f423c4969bd9e42b6bf688306a7b987e9c3a8c70fbe547a77d95486a8455eddb66312f2049bab" },
                { "en-CA", "4842c5e8e821cf7f649673e55c133fccab8ffea7630c6704d6cb3b120d180f481ae7adb9922a344135906d39af0058c8e58e0442f798a1b6fbb964ae37aca24e" },
                { "en-GB", "4a564ecb0940bc7a76e1537b77f7f9417e426f032b8e04b12717079fb8b8d3180aef85f8335f383a6d1715b6fa22713e0254d31bd71ce3f4781fe0818c66aa3e" },
                { "en-US", "e3e88b420c7e10d3c61040e1c07d0c982cd1abddfeb5e2aaa604ae387e6fd0a53bfd0699bf364d6c4ff3c283edff85567b39165f39b6099e2189c22d6d0e09e2" },
                { "eo", "5820f937bd7e6a83fe2124e61d22671ab00e134d1bc961ddbce8a536185e065d68385aa5545d53fc643d15c2e0e6437b840275ae854650b7103ab1f494e69b29" },
                { "es-AR", "d6cd9c936341410aa2e6ea5632d547a4c571e377ad2ae124540ce07a222158dbcf5a2f333c22346a5c508d7487789c7d2c802eac29e31002d513801ebcb9952f" },
                { "es-CL", "f3bb44b5c722d769d732b0c49d515aa56f72e1dc6be8d6fa98d05de23b54050c1a94381f3543c8cafbcab7764d75eb3dacef9dde9912d32538910ef83ac33156" },
                { "es-ES", "823f5fd6d4ecfb3a03b1e334bdde84ba76808136b1afd576d227d5d6dd73dded82c41e504460a8f1ed33682ca2ba3efe7de132ee0ced0c212d9ddb617e0e1533" },
                { "es-MX", "3eb134169030871364021450763467893d46f80256d2555f9c089b28ef0bfb39c729850ccb3ddce189660a7c272c8ac5f38cfa9bc70b1d8f73aecd988ab38ed6" },
                { "et", "7e324cd974e8d813a7fab272ecc3ded27dc96fbabda1c272ed1c2570eb1541d595304c7a7cc4e1db1d74a45f2e8b5c332a8d53b38c736f7bda3bf0433b4c5939" },
                { "eu", "cab7c5f5fff4375ca3861c9c5d0dc2fc5801e2a25bed59fb902469996db761faf8ee3f62d6fefad0fb00b35eb3bb2e78853235ee99e84e3828573eb55426bce2" },
                { "fa", "aa5fee2a48564383d61890fc7692a7e9bd2958d86018a5cd30f20d15121c84ac3bc2f1e5c373caccfc9af452271ae261ba60a1f1de357bcf56915c143e32c164" },
                { "ff", "f733d6e511d0e40d7b6b7d953e9a31413b364fdba1364f742912fa8924c798ac60dcf4225b273c238f71278a4505d3e75ffd75bdc9a1500488532c284c219c20" },
                { "fi", "3214e627a87581a97f98e6783eb1e8c2624dce08f044f7fda3f9ff621a6f4770848267076074b1319f5ad9747991dc0dab1a65d64c0acebbfc4e3bdcbab0190f" },
                { "fr", "c7082f48e468b22e65c8169492bbe9d786ac56c012acd0a989e3c83d3e9bd8fa61b98fbc957836253d441ac394e45302005c17b00a190cce7f81a06c612cb959" },
                { "fur", "c94ec7a0b1ae525e207c5cdca6abfafb4cdf0d54c03f0d139bec9eedcb2686bcb9002b74ed2b78f335da1117181bdb4963a61a1a3d7d868e82f4172e410b8269" },
                { "fy-NL", "c9a34c138be7111ed94e115e8fad2a19a5c9c7a2406d3f5b48f13782346390c4e18365a63591d20978766b8f1a449270842d08470155e282f2fab5e6647c4761" },
                { "ga-IE", "aa34fea84dac9c9f7901c4dbc9c2b9954c1857be54ce3e1e1769ba1befa89ed68ff64558e33e9e7d50ea2e4dc3341e7e30f41e21162b1e1b97d88902a99cc92e" },
                { "gd", "a5db6cd89c39f39ba5c0f0f0caac372e789b7e1da3d7dd01ff4a224eeb392d244cfb445a077753c08b4583ba0cc3cf44154f60d6e6f415c2ae85aea46b58faf5" },
                { "gl", "f6fdbfa35ed7ae952eff17412f171808f5247b29b9bd1480a54c9c8bb28734a0eb8ae5129ff5460b7ca1ec47274c275b7d0b9e63d051ec6692c0fec27aa5bb65" },
                { "gn", "fc7f59243cc19065eb3df694cdb9c79dcd5c990a8689daa326ac38e896378c6a1b68b9dd56bd3a8210f29ff5afcf318933814d58e6ee451c5b400a659849a7c5" },
                { "gu-IN", "03e4cd4533a4f897352c8fd43a406899e1622a2f1f596121e0870d42196efd09152da78a7badfab41a25bfb2a710813d1d8a89b54f4cac7dcf13031160c4514f" },
                { "he", "1c041c76a328f95fe29590bf0124bb893180bded4370e4e2843629b7cfc40e96751417783d42e9af37c4fe686d183f6c4cc2eeaea2cc27f862cedea18ed7d110" },
                { "hi-IN", "d665c5bf854731ab9a779bd0045e29eae32b84c0ac90f2fb6a04bfdbc37203dad5b8468ab2e1b706e7549202386eb864a03ce4b89340c199eca2fec83b7889eb" },
                { "hr", "d7b9be3b94ac809cf583071c7f4838dc05593b0a6518fdf84230335f1fd5bdf4b87aa68a969fe714a3c1cc2d834601fa6abf9e83046598e854a62157743ad79d" },
                { "hsb", "4c94de9ab0922b2428dc62cef7272ad97d99503a3d92550974b8b42de156860bcb20a526acd8e1ace090fcc8b2ebb3dd3554e43a58e5c51c15d452697200a5ad" },
                { "hu", "290f944db021883230025e54b6de8f3036dd2b0c312c145646b06d1e48b145546b6481f3977deea928e6c176e6963700ad2a3e083679245a76071027e4fd312d" },
                { "hy-AM", "e0896cb4e04fe6430d52a73d5973f98497330b0dd2e8d15c2fec8f5c988901b849daf49972f6a057bcf3d61ece2f0c0c0d38aad15aa1817a7bd30bcbd5880cea" },
                { "ia", "9ae06feea00629fbd269a6fe0459dba3cd2ffb6ed63ce8394a5185b32718c5ac98b1d1a3e212e9e699b61b9f969cdc6a07b4744f4c0ac75498a62b0d49aef1f8" },
                { "id", "82e35643e47c7f8ddd769dd9e6352b4f608876d6be59b4a60329e3677fee8e037bdd19955170e52f70bebfe0c57de884c263a407a6ad75f70661cc0f56febdd3" },
                { "is", "5a6b488e2f8e0ae7461bd2614d3e068ebc74b01ca3b81e824f9af049faa26e0c84bc19ab5d086bbae48f405fefc73c8abed51f7cc7d5126c252f7354d1128520" },
                { "it", "bf294375b8c7f7276ffaa3c602bcc98ede188ccd04ed903145dd9a7c1d672485cf888ac43b4cd9492d0b5dbb9bc0bb683112b62d3f2f6f759a92c28c418b202e" },
                { "ja", "131e47d4b66cfc2c7dabe64e79450f1f436a706264bd644a9951021aae953015b1cb8976afc73a1113f0a870f46d9e23ac67a63c93a71ebcc2b7b5e6eccd71aa" },
                { "ka", "7a03670b180b0a435e602111b4eb57f184a79113cb1241bc92053f6bc09fb26ac432169d0e52518f5ef82c164c1afd760fb20918f0e5f40bb31d48a00ec53a62" },
                { "kab", "9f84a441c1137129c0fe120346a5e274c849cc271a3564bc471fc53d6bb77d314db6904a332bff29ea7a8430e2d182bd4ffa354f10ad8ea405d88e5d7e575f23" },
                { "kk", "c7ca26914659480a06b17d1b1b0896f67aa925aa5bc1e96cf23a655c087999781cb16032df4ecc1e37a3767ebda69ec4de06e36b82896915d228d9e86f0ff4a5" },
                { "km", "e157f788831fad0709d2dec7af3d98b2fb4d5d146a348a939479e61dfff74d707f1770e897b694d9c306185032f66ece41fbab880027f75f0525368a7685ac21" },
                { "kn", "9c7c7d097a5080f74b2468ce888da9246aacb4efa9ec90842d80dd664092e3f0a33e1ff1b3d5834885953007b29b790369055c86b5b3e5be25cda5398fbfb00f" },
                { "ko", "7c19e723b59bd22f5d3566b8a2439568368c297ced013cbca632cf29244775e71b291237a5cbf10fea43bd087f664c20502e83837bfd5c220da11626a3c82264" },
                { "lij", "ae3a6cbadba71939eb12b9fbb090576ac7c8ba834c7a634956aac39abfefec6797aa532cdf8dea537e875ed94bf70e452338de5d48a970d140a78a04ada71994" },
                { "lt", "dd44e3c48361970b607a4a6995d9c07ad3dda54136f3df14b4b30bff75ee4da70bbd5d8bacb440ce0bdcfe4e58e283efbff52df33bd394e7cc071cd988c7b412" },
                { "lv", "db33db2d297db86ae3a70d28f36898cbe96a1de69f6dfa74835336789ff13517dad4399454bdb7762ad2a06154947b1a2d8b60fcf011177b79ed34611047bbc5" },
                { "mk", "f8a352f3d29b02048898c75f9502cdfeb47c75a24424b49008bc4872a8c82dbaecb396f5da1746364358573e4a91ec332761e69e8037bace20627b2528396406" },
                { "mr", "afb7f1580e763d59e0e11e5f4e2355ed38b57e10791758c6ec7dbec19ceb10a89f157ee7de5523558688d0ef57054b6702430819b6a4e834b80801c67d77d089" },
                { "ms", "1ae5082d7da316376222af4df26c1a8b28c9170c7b14a891b189ad45389c7ca373f6d6a46a7662c25db093c44d31a0e1afffb5a34e5f0874a531333aec670a9e" },
                { "my", "b1430c32a9bd1a2df90e3d6567a870f5e3618cb223d7c30e2188968d6c2fd3913f356e785b9ec4df0968ab4feeeac60b7e2c28c6d44af7c53174489b2a3f98ae" },
                { "nb-NO", "4dd75d4eae791bbdcd3dd75aed9b78f21962e333f1daa9769d60c08e1364d4a96c7fc114a4e975a6572bb3a41a8c43d932c2f0ade64338e0fa7c9b1f666ad29d" },
                { "ne-NP", "b06893c7278613c8d2402160a2a98c806398a9eb3dc029536ff026e1084cbfafba7e85b5fe40ef9e498f4d93903a1c4c91a9571a34ed4ff82f2498bd979aae92" },
                { "nl", "15828b10f94d20605df4cedbe00aacd4e8f82c0d83807d2cf38a8491e4f01784187b8a66e960ec13d9bf6c7b5e0f212f22d345dadbc99763b96494eeb8467361" },
                { "nn-NO", "545e87381c26642ef621a1db2ce2f6e9931dd247d267b76d38e32c40afb5ad3a93a4eff9c50e41271129b46354939a03c6fa11a510bcf4eef40ae4f5ff52ef33" },
                { "oc", "7b4dd72199f6ca162bc7693942a33998ff1858e985292da9bbaf17449d96703359ff48b78534e156ea6394d457feaddf7af20d7e67a8223c72098f2ae24d62b3" },
                { "pa-IN", "ef807b1b71d51dbe7f87fc46b37b434822d26553ddff2618c343adb708adb717f6f715fde837dcc3cf7e19fd93ff96d977ddf2ec6c0d3db91f486dfd10276cd7" },
                { "pl", "d961bab4bd34c5cac7ce3493500df658ba6a6bf22fb45e6e36c745ec81c78cbee8945d36b43f40dfa4ecb2a31b6a61ccb7f8ad0616f2dfadb82df3dd1788b74b" },
                { "pt-BR", "84e8d976c61c6f6a8d8b4079d4105f8a5893cfe7c0c0bee69077fc2d7b4d5283bac02fbdd0bfbc0fe456b35ea64a1152922076f0e6fb7d1e5f9e60fd8f03f41c" },
                { "pt-PT", "348ec0c0c00d45b34e14b709867cdb2f9974c324d5de0877b3903518559c3d5b2e63285e3f42b1671477eda785eb424d3f1623b78ea643b5ba7c883fccf7e259" },
                { "rm", "4f5e09e102417f3dd6eaa8e03771832eb7793a22663fe57c687988c44bf8488549012396ce2386a1f3217be788c859b5cf124a526e16c789387a431ac01942b0" },
                { "ro", "54aa354bd8879ed4547acfec3c29a8e0be9539fd112c094b77eb04757d3f96b4878ce69f739ad02d38c17a39381b1877397c20585d23004c477dbdf71d82987c" },
                { "ru", "7f89600a02d4b57b0e56f5ee66622e040204459fa6dcec9b6746d506325ff7c3fe201a5dd53d44ea3aae71c4ebb219bf54925b35f55d8f34433bf9fe6f2594c1" },
                { "sat", "652df0a213e9ea99b780249a8fca71460965a669f4e465b0848bbafafe5e5702c815891ddf508cb97991529983af69b14fbbf3f85c877dd817ba7aa6928e5b31" },
                { "sc", "14e66003aedb9fa8c3bc5a8655705a292795078d1e02c1ac95d5ddb205e9ac15600c82adb03943523d46137708b93c1a6b80b6ff93e6cd43e476e2da85302216" },
                { "sco", "ba564702088a85ad5ef2157692970d4cae8fa2e6fdf3dbdb3478c6bd1fad6cf768620ed9f23e920ed7b5a34228b2ad8af335202091b10fcdc04d415e4d1cd53f" },
                { "si", "9958829b7ee0d2152b0fb817697529ab62009267a046fd12c38b0974952b335abf5c9881fee1d790e392ec9db753cba120934a2e46d9c0eaea8fc7e95380c625" },
                { "sk", "f649dc4384d8c0cf88ab5c2b5e1fb50b62e6a3c1882ed15a9b9081568fa54c83a80290460287dc6a51c3608fcb86756745d95e3edf44d0173ab4ba713134342e" },
                { "sl", "ccb876a744c00b2d5a89d569647481bee182455cfd102ed5c8858f48a612142911c4c4c151891e762ba78e15ab8a0863980244f99474ef30c7a6498bb00656e6" },
                { "son", "1a2dc6ae42496397a495c7773b16cccf75654baaf7ac62d672355374c685240af4f5f1eb8c44fd10b260ec0a4be3ce788562f4390a82ae9d07e6ab52f1ad58d2" },
                { "sq", "cbb51a1479100ec7f24347e65c2403ce8d9f9062c7ac93e4c33f669e99d1031527dcfabf3bf4ff3f2af2f0590e24cca0d3ebb8a426d9e6323d25aeb459f1af75" },
                { "sr", "46c4e50335edaa1174d12050c8f91613e883b11482c192b9f1ac44124c3f7db3bb0e760211a2b1a024e6eb618fce2e8139d51e4f5f38a971aa536c3dbb464473" },
                { "sv-SE", "549a81139d065c918c5ec758331670c87d7b188b3628fc18a1b7ca7e9badee61b38df7de9e4d52a171b2448839335abc72f8d996553afa516501c5a4b8cfea8e" },
                { "szl", "0da139defc9f65d8b34ad15fccfcb9b7d7bc450d74494f19974d9b06855531e4bda4acf8ca12e9e5b6a994c9526b909729fa0d2766aba237b7e6c68a01b5dad7" },
                { "ta", "db01243c581c854481aab9144cb9c14830b8544ad26e144a73d010d8034f7254af7dff63fcb181c1c65934bdf48c7127e3a5dc4ccebe11cdc826ca8ae2c55b98" },
                { "te", "048c36a8b17132c8e19abb1b875e9d6ae695e2838cfea9128627fefd4a23c37fc48b7f9e9408ef44203a00dee792a218b23ff170af49edf37c0390846ac32b91" },
                { "tg", "5509aa080a0d7c60ce5c3c3dfa295a5d0a6c2e104e6f62d3228e533d575e2cfc55d3973e5187c7b9765fea6a2761012231032d5573307d8e7885284883fe2b6b" },
                { "th", "ddbc9fe7770cb7ad69544fe53ebecf27b1b8ef177fe8016d83bb710ebb0e5305bfc368b5eaea67da60062c1218497b6bbaf4993f1e9e6bcafb7d709d59ed0823" },
                { "tl", "ef6e6b2bc73bc2e9e3696e02fe5bc2ab074ccc81c8663b591cf816c2b8ae6832d07ece3f31bbb9c9f72fadd4dab7fd897ad9c59e2b8e158ffe36679d5702ef4c" },
                { "tr", "fdf047490ec5f4d0e533fe00b543faf5c826ca6c1bcaa2b56c20baa7003a7ef51a9a35250b6444c81b1bc1f616d7c9e4d1066d598b13caf26db1eda10482bc3d" },
                { "trs", "fd55bdf19df64eeebec9516c61273c750dee5a719caaee5a1f4cd04f8024ba6b4b91912502baf41f92bf21913059658af7ba34bb8c43dc5af9cfa55ff9480170" },
                { "uk", "8119d38f8df800630b791880d416dd0754c8cef3a7af437554ea3f3913fb7389717118a8183fb5a0d9072fe860027450396e2f212fe7d2cd369c4c29d92826f1" },
                { "ur", "e59bb660e20d2225d4f76d629015e657a75c775c29d067e73570a7191ed1ab6091be15984c70f81812220319593eedcef8427933cc5f66b58cda7aece2e835ea" },
                { "uz", "c00499c591c3677ba15f32da576212ad7f02bdff5b3409d66fe785de867615123d9959ab97f436e9565aad0aca3430e94bb895e0dc76bb976f6acf96973125c3" },
                { "vi", "570ece7d62245c87028fb0f74eb0d295c4c6a80dd90570ac3a9e94d30e59c9ac0859feb39acd60ca3ac500db5a48d24a96c9cc7d8141dcf83798cb6117e9c726" },
                { "xh", "0a3fa78b5720754a7acc2e51cc3857cad36e409768c8239c1bbef9172ab38582f5c96ae692434ef3c7e5bb0e7812fb9fa751e3bb49789c39dcfbe807a301b4d8" },
                { "zh-CN", "c6e6196f44c90ffe92baee474cba8e371c199517686dccfd5b4912359b1ef9b6d727efc71b00bf145d7f68baf2ba027a2b82dedf251d7cc46915cb4badf20709" },
                { "zh-TW", "df9e84ee9e175e1feb8b888277fa0f24213c0d6d42504734b0bb59c3a38bfe08f29bd2ac905c48aefc63d8aaa86ed8e812562a6c0f3d9298122cfa6d58761d94" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/126.0b4/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "bcc15ed8f35c5ffed126046007f564bc6a7a8af3033b8071fc97551dd389015850f53e5bf7c5f995b8396c1ed48dc6e5cf88a5c217252d89cdf6421a4c61f26d" },
                { "af", "c0fc240bdbde5ed13d46f0cf35fc60beb37b5aaebcb8cc49a2b835839e508b4557fd8b14a79bbd5c535b0c179f522690f183e1e7ae5154d033a4e405ba96a1a1" },
                { "an", "50cb48e25b4bf0437025f8d9c66176752bb7c9f963958c88fa70ba5a32deaa13c467deb464206c25c938c4b625b6dcc98a6b1947612926096a52cf1e562e4fff" },
                { "ar", "8240a9f32fe816b43bbbe9505e7983c8d54a96b06b07fbf1c66ccf54dac904bb4dbebe61dc2cc0fcb9bd1c53dea79f58d5a6f888c5e75ff30244c376377214cb" },
                { "ast", "011df8df289d8f0d8b5f1e761a1bd2826e62f20425b866947fa859546e3c7e6545bc57c2db554ea488987bcbdaa49e077890b0035541cfb5e6ce65949d350b26" },
                { "az", "0617deb06aa8b048956cd7f3f57dc712c04ee8887ce4e195886bcde90ee1e688341583bd48a1ea1df59b4f63ab3ebbe5681ecf85eeb9480c16a73add14353151" },
                { "be", "1d299ac39ea4b448af881f56f725ed20b186dd71435ed8f0c2b4de46ac7e6108714d57477d793596149ef23a3c4dfeb0c3cfb9fb901d7c8c55b2628f6e255f41" },
                { "bg", "6bed2ead14213d1a9d187781f384a26f04be61d0d5fbbd1613ec299899298fb962cbeb83ff6ab2c32c97adc318ab7f8b53764919b0572275e99a22b13d4e0c9a" },
                { "bn", "34aba23cc5990090cb654235b5aed1999374cef7ba561902799d10336a63abc761f89979f2d9317fde3f10bec62b57983e92144872fa7ecc8d631fa4b0ed68cb" },
                { "br", "199a37cd3baa6d3ef149299348216cd21c85e2091a2cb654a0219c440a0075376336a18d209ac1c43858b03e0194f58f04c3dc44e4f18e282b24cd11534d47c4" },
                { "bs", "8705aae93451aae6fa4e4c18973f2baabe2670024b1d403f2f5f1e02033efb44a407b4d359a425be5bbe5326e8171f2b4fb7e5fee6135a6374ce0ec2f9178572" },
                { "ca", "81f5c24072a571ed14b55cb6e4cb8c8679fb5c3ae9245bf0886edbdd15902b2a753173a3f7368094e5509c07245479c1c98c9287203e74c9f5efb49d452d1449" },
                { "cak", "26d979020ca1ce99a5d54c1771af7898adbd307b9e390542dc5ba18ea8a03605f7126245108b98a75351cab47a65443f4e73fde3eef3ace2b7f080ae9ea6c576" },
                { "cs", "b5cdae38db8b0fa2b53ab22afdb354aa86fdf848dd157ee11f9be9d605bba9996ce68ddf13715363703b8cd203260bc742b4eb476feb7a4abaf572d94429ec1e" },
                { "cy", "2f9805844679b2909689d7096e6a28477676a17be0c4cbb431f74d1b1096ea4b70257d64c701cd6182d919c1ce874c370b0b3ea496633c51d56c861fcd1c8304" },
                { "da", "3c5893a85354ab3c3ae86db012d670854526c6f8dfc16a1b9286c8e416fb6c37eeb02be222b71ba64e51a4aacd61ba78fe420affbdacbf8a27f6ab17cd0063c8" },
                { "de", "771c2d82906eb7dd0f8597aee36ddee9be987fefd2b9cbc160d94e6eba6bf064ce23c839dc5052d4e08a73b6c94d5b93f0c646b0dab4ee7fde9cefbf2a4acfe1" },
                { "dsb", "482cc9f9dcd0e047b508ead594991f5987ae354c181d5f854e208127c22206333d0eec2fa175bd2aa4618f7dbcc34b7094b7518692baf28b2f86ed8e5d462113" },
                { "el", "80fa522c22f27064cdc6a612e0297092ae6ef390ae1f2573e068d7c4d015e05d8bf52013f4213b6d8c27cb4f9a903891162da89f98f1d76382e5528b13687377" },
                { "en-CA", "4019c5f8414d9aa92732c464689fa9daf3eb69cd938c1f0e5e162e23106635250eef7aaaa34cd03bcf182d287e9feacb9b7f05463ec0561bee73e48df121eaaf" },
                { "en-GB", "dfad2bbe616121a560fd67a53c81309208c62cdd22b8a165da816ceaca62bf870490a92eb8e1ebde615f78e923a3b23c5430cf1498655518ac8b0df4881fabeb" },
                { "en-US", "15501aa97e829661b067bf1643825d711fd8fd17b1f38af94b7af1110547cf0920b4ecf8e99ace0eabe117e7caee35e72e1bd6328e1aad69351fe1d4378f10ce" },
                { "eo", "be8e803b04cc893e3f66127240aa3dcc412f7620567fabffedafb12f2704a84a8e2aee590650317b026478fa9f736af40ab7daaa5b5662a9c069cae54cac5265" },
                { "es-AR", "561b38a93892555850f82aa2cffbff72a25e4aeb6d0afc4c595c36922df6f3034043bdff7329cc18dc91cb365f1cf9920a72f3c5f3917cf0f88abb7e12b08c7f" },
                { "es-CL", "c7f5f1400e3b90d70df7cc6f46a0e3f2478b0dedb32ada82d206d08857f8f271b02d98d00708a8cabe698b2eb4aa2c44fe843dac06814e3f76785496c6729671" },
                { "es-ES", "082d8ee025dde5aaf0607f36a75e34390f7ac7f1f2357715348a26ae9c8acf0120b3e8b1ee88e80d995475e44f26183fd46f4a23501c2cf199e7080a60d1d6de" },
                { "es-MX", "8dc8400d8a700933312f33b9ac188ad2b3bd66e8d1f7d2a5e0a2fa2ded11471f694555f9488a0f6ff9d7d682eef575d15eb90a9aabf580f5301f9bbb50ea941d" },
                { "et", "583e7870f135785282fb31edf231c136fff1a84ee8097815617b58a45496ea46d9f49acfcb7b192a40feefc4a603c1a04a0998934f9ef1015f8c0fb3226fa0e7" },
                { "eu", "9371d6be051bc03356add21f0321ec6e53283672415e55d41e9753146f72d00cddc661ede16a40c3053551fa0f022649a99caf968c629e42f50de8a1325e99be" },
                { "fa", "237819e896cf9eef87113a743ae033a60b0ff820b01d3fa187680812c04a6fda3c8fada56e3cb4f4426ac83798014264f8e9a80a7ecf48a1785b4e983de2bedc" },
                { "ff", "495a5b5d1ba1313334b4b2ab974cc8a85a8bf73a9b127b8db2074bf18c94bfcf6431900237fa11952c8866907fdbb13e815c8cf1d3e49a97ae9c14763ecc82de" },
                { "fi", "71954d7d7ad226662b287e10a1e42999f51428dee4125acacd83057a8fc86b5f48d2fd07d7a421f9a4d6dfc68d031c2ce2e91e0efb5e55d764512fcdb3b4aa58" },
                { "fr", "72f76e12117b9bf08a4547f49f81e087e0f66c9525b371c54203e8294236a983e6462ece513aa3c4a03f4996e9540eef40fddaed8d4d120b6026f5a5a82997c9" },
                { "fur", "1af9867d98bf86a20a1ef83d30d7be99f559f9294f16fe73ab7f7b0ce4ba973d1bc98841e3a4904155894720ea616269b51e56a94d33598732054d72b06db881" },
                { "fy-NL", "0afa934061d5218547722a90c5097f225418e3f7532477c70b1ffdf1824d8ffdfee36a7153b08fb0d94430db6ab77e48ae9d3b33af8478e652caaf8f0d280984" },
                { "ga-IE", "cbfb7c109933e3117aace4a6d3674a207f11ceb8e68d550fdc126dac6cda9307a0251d7125a02e50c6c8b5167d73b8cff00a62d10c230293ce303b9c78e5f3a7" },
                { "gd", "3e0dc04218289e88026208918bd69724e5b7fd8f234a68f8a38ee3ba43bf113adcebd6d894b5418de4b1c85faa69704ba7344b8f6ca3db8b77ee8a0b20bb731a" },
                { "gl", "3e3833d214a64c04e0bf8f9d23e532ae83b0feb8ea8dce1a747e34b0e9919259509ac339ba433d3e29b996e4cbc52d5792a6ad8baaf2fc1efff868ba88bd38af" },
                { "gn", "bf89dfd57ffc73ca7aeeaf56189203d9a2f72febe7ed12ed3c69a9eede046e4974f674319ac2895af91f68419f3c43a0fee52dda0c3ec4856dd1d3c06956dd6b" },
                { "gu-IN", "f47883883a5feae8239f588f0abbe6988a4378d908f1a22c2745727d44a6c037b72d0ec8dfde5f0d92c488b04730ad5a50d4a8c874d9654003b2facf452e44e3" },
                { "he", "c924414fe48ca28c5ed4258dfbf79b8b328325009d58240f8523ff80d908fc63b3bffb8d75a6a591bf382397b6fee2d7f64209c0e5fcc2863e9a4f12df371a24" },
                { "hi-IN", "fe4a480f1a22969df34b056b6f15615ad2eedd647444174e616f10540f56b59c5df62114ebcb22bc0a595c8ab0eebfd69d61317d88c2947fd14c5796781333b9" },
                { "hr", "ced571f5816b93a4b8425251daa9fc4f6f1f91c49daf738d163c405029a4486dd87451656e68a8c70063f28fcee29c932438cea0f775a806c90016783c9a741c" },
                { "hsb", "caaa833c9b4503c018ec10716bd009f4df9cd5ae5cfa2788e442cb98abd4686bb7130896817ccf53406464fc54c6e98b8b0b72ef5437fb8250fe23afcbbf0f66" },
                { "hu", "2f17b6d7ef9bc4e991d0d267c3455a8cf2808fa802c2c828194bfe73435fadc70caf92609d6c1105fe724fa8e193e686b5270a95c40eecbddc8d8fcb43d9357e" },
                { "hy-AM", "c7294549da43243b7b5c002cea49abc00ebd91d656dbe97b5d2ac3d2c99a1903aea24d5fb89220f61af4688d8310a166fccb393d140c9ff38fbae87c737e876b" },
                { "ia", "f830dbe42a0ca87230c70bf2c8c0b96cdff0e01acec012597b414fe1fbed1f77e62a3de402d49c11ca04a11bbb097b952b0bcbd3a9c89c3b40fa95d4431313b6" },
                { "id", "a52f889b27b99045868a99754bb04ba0cf575a76e7634be145101ecf6bfdd1c82140713c88cc60cc521c0e9550a0350e7bf83f5868c71eb23f3f87e662dc6866" },
                { "is", "5e7d15f29a19bcebb7903620d4675b3e8654932a2644a98a2e7d53587acc8a98a32e89ffeaef37c73fbb0e896a54ec6237b36570e74d2d1989f33ce34405d24f" },
                { "it", "aa139aca13797bafe8dea429bd69ffad0c810e4ed09a8d43a5d4fee566f5824cf2dd2e0ac5a2a137cc47a0eda26e42a9617bc509405fce744695679e6586e8d5" },
                { "ja", "e15371cb0b5c9f8e6d1399bfea3cb6b5722cacada298796873b0ad99a8d294acfb065e1d6a2655a93563b75f57af604dae6b572c397f629f768ec1d2d3946532" },
                { "ka", "b08e4fe45a77fbe273456c8b1e18caa039ba274e894028103254956e8c114d51119a9844784a90b2e8d5494b4adc5d427d401750c438d322470985ae6e4581aa" },
                { "kab", "4c4d5d0c38eaa071e5c9f8c1816824735a06b381415921ac4c2251a5092bf7c36ed8e5a2f8e515e02f97180cfbbadbfa1ec1116080b8073312319895a28e99a6" },
                { "kk", "131bb96387624a9cb9b8d0a3d051fc0674a3faeb3c5169c1a527517748096504cf5e2aa2d1d4798bbd880cb7fedf6f924cb1402531bcaed023514e31582e5ff8" },
                { "km", "b7cd653101a22c7b248b875292a3cabef148e56f7358d8aa6c09dc1b65b2f3624229a743faf74d1340dfff5af96a67b1fdd0e6f11cf88aee694bb5b94a448004" },
                { "kn", "4cb3ab2f22486b92e225b7816bb7dcba82c712125bb9f5b7afa7ee1620328233cfcd52e24189ddf10f1d7db425ec5602ebebd06ea64117bdaef7e92a16abbed5" },
                { "ko", "fba0c70eea068d5bfe1d90ca5dd174a054212f07ee27ea3c4773fb67332b091cc37272859d850f49c119f27827ace4f81687dc70b874fb621211ba695e16a6cc" },
                { "lij", "ac68b4932e2963862376e58f10a6861d9b7225aaf5f4f9fa7fb45b39348c9dc3994b4709263a217d77852db0ebdc3fa43e8e38abb812719fdf2a0ea44e40cfc9" },
                { "lt", "eb1008fd30aa1079d6a35bc9cf2913d3147466f6a931cfb96964bf5f2524ca07d452d31fc69b5d5c7654a6d1c56d1ad520ad8ca204ce34cdafbf5bc97278f712" },
                { "lv", "413d91514083ff18f436e2628f8b56416e56cdbf45d3be900974082d7b0b261dbfe5e449ac420b6620c6baf11a0f78e7068f7429af41c2970bd5cd6af2d91c78" },
                { "mk", "8ab518d0ae355431759fe6d2ae1ae1e85ce9a167b51549211a48b19b357ff87511e301197e03fe1291a08a4361aff88b8610a4310e78f9a8743872ee6af74333" },
                { "mr", "d86c1735e52425e8ea559e2046ccbf9539cc9b02f41e2e1f45c7e2b291686698b5505893decfffd2d5a8778d58ec88266e5c53d1d42fc1fa941b9fbaccdb6f69" },
                { "ms", "26b31a8ac4e8c2a20a34332a3ce7134e100255db675015ead41162130c8bfafdd1d5a937a45ced530e519128401c553bc90f930531eb6542cb1bf15d3a75a8f4" },
                { "my", "d389b15b5a6e52adbd87906c29af067cb085398fd3fc2c2b416d86dbc3b1823774a1c051e807406219298096b28c6ca4ef4e6e045cc1879e7b47c3a49e223a05" },
                { "nb-NO", "58345ad7dfef63f3d357572d4cd636011323fc7c53a251b9136bfaa24288c18e2f95b92def8f474dd82bf4d3fe349b19b45368e21c725d923dc80b63b8d6fc5a" },
                { "ne-NP", "a975933bda0fbf5819437f52f740416a035409c8d95dc3cb91a935818eaf55c7dbb91f68500484e1295a92f7223ad2f0a5828dcb44fd9721c73d61b419029243" },
                { "nl", "72171103777016348fe2eb91862d9f2a61ecd98e80e872180abb94f87f80fcbc80d2a6bc1a7e9f6f044823d46edd6752979df6349b03570b9786b2a32adbf1d2" },
                { "nn-NO", "00796f0efdc8ae7a72f47ddd0c82e2b3f40eff599bd327989d72f9b01de6bda4530ee755f5111b5dbe6c15fb767f55941b9cc39eaf02dec77a5b42b6e690693f" },
                { "oc", "77e1b852cfcca98dfa3a4ab7fe4d8218ec18b2c2e702e935a358a4c77e3da5dd07ea626229998281f63937ae077b873cea879e06af8a22378fc97321c4864329" },
                { "pa-IN", "b74bfa842c882f6068c51181576c8371f898d8130dcf714b6a1452ded17bb195e2474c391b4fc1e778e6d0beb872dd1585ac4e0c6f427a6e141e8935a2acbd7a" },
                { "pl", "8a3ca855b0ba69af28b7174e08ea2b436677d284df70497a4832c33a27ed463c34941a62c0ae8dded66126e1051cf3a5fe59d0c284669acb1d89bdd1a11c6d6e" },
                { "pt-BR", "45af4df230acbc52e4265787661ef3d844f2c6d55664aaa1696cfcdafc397a1176b97bbffec084330cf2eac2f0757a95f8ed0d694f209287952e9a8cccc3e2f2" },
                { "pt-PT", "1da1e0ac923a0946fcfb94642e36e5fac836f12f376d9bf6dd0a32c53f82a0d6572e9d24c237ae0c29861ec5e95eef1e3030bb142c23fae23ecb5517e408dc2a" },
                { "rm", "7b01f8d2fbe2ca151804164cbcb7058920bc02049780f07878c4c0fb5087c747378436fa1a895cd447fca47c569dd0a67c6513de1e5279dc39bf14822a9c2895" },
                { "ro", "b236327aa797f5914894a1a2d2b09b560bc22732a19a4053a0abefc7a13ab8c5e7fdbbbbbd9a8427e161e8de3c5a6ae356227eeb29770ad5e92594de3aed0e8e" },
                { "ru", "e8e351d8c3c842e6d62d119e38a43a3b5ac8045d25263baef337fc56507f0ee38ecdd3b0eff6a3b008f9539124a0f03f5141c1ea1cef987107bdb44a5027fe99" },
                { "sat", "11eee44ee53cbe6f51155452d6f8b00336b7473ecab836158739b18ac337d24d2a2f784c0a76a83b1eb3801f18c402cf61829a870e89739b968c0c8365a6ca71" },
                { "sc", "aede51a5f78dd8b03037ded34a36afcce09c99b31fbdf4d44433d16eb1bc0713a6d481ba2ff51421f45573062816c7de6f4a3522ef55ece93c3fdd0a5580c510" },
                { "sco", "3d0f08115cee4bbb72946930b13d0fa86dc4b7f00ff1b27151c36eb7f8cde3b668b927ffa737b7fbb829922b716fe6743a8eb7902e03f6cd96a7313ef22a6143" },
                { "si", "2ff81432a8b55ddcd679feecff7f5d7ef1083c18b075443785c40e8a2d876feff490bf3f584206c44f30ead83fbe2be9d6258e416b8140c69c8892b9d58f505b" },
                { "sk", "c86b7e007906e8c4eb5e0cbc64812ae99bb747edb57be987a0f4a8c2a527cd6058411b2c02447681243f5a2395402537f86c6ebcc6909f12779c330700a7700c" },
                { "sl", "9da00db40c72c24f200942eac95bc04678d9299bd566406a6fbcda5fb8c17bcd12f50fcc638a91e5c985f362a7796a7efd1b966b03933359786ccc3e4423da72" },
                { "son", "d3ff5a595f1e541daf77a06afd519de034bbcface95d4907a24bd743d23e02c0b344e6a4925e750db28890f226ae54e379ae157654464af02b203b282c22a405" },
                { "sq", "38582b91054035024baad51dfb0cc2bfc42aa903725124016fa9f6e747170f700c53620a4347339d5ea8811943315fcb138c4dbf9bb6b8eae8f698c60cf9c1a9" },
                { "sr", "1f898b181086b07cf5ec58881be884117902e9b6e82c6a8e48d0b7bec34c9a036fe0a936f41144c802586969752e23a3f18816caafa05fc9a1f7a87027b8bf21" },
                { "sv-SE", "090e386e52229ad8ccb7b120c884699b239d4203012521d820d8f00d2ff33e3816bdbe226752ab82808700ed50f4cfb677f7da57ac8e0c1c44bed2346e4a87dd" },
                { "szl", "f2e48dc1c85967e1059b2ab473aaaa3ce1c00df0b02cb60acf284d6910bca3b610f177976992284c2ca0af99732c44a208a75123fa41043c3a0b933ea4847ffa" },
                { "ta", "12927356b103bdcc1f76529a4555f20514a9aa096c5e9d2f894cc54f3d5ff2b6ded2e151d520c0a7c8150bf9b180195b9f31949200e84f4356323efaf43cdee0" },
                { "te", "4265d88d1e0bf4f128ba757a72abf9d16ad90ee54591ed39ec349cd2e136e5dba29aa92d4d3ef80899d8d6d833669dc40316e8ce5207af715b1968db6ae55f87" },
                { "tg", "005d04701671c948ab2c9c8f2f072b9cffac95ffff98322af7f83d1ab1e2e26de19680ba2c95825d01936079f5a34a4ad06fb8e7d6e2782f8dd5c1cef23eb814" },
                { "th", "c809023426223c40297e70ed21c3ca93a955e56e6873b753b715bcc75023128ac14efe94a2139416c649e44ae63a2b575549c842073e954d9ad7f08a6e7247cf" },
                { "tl", "42405b477dc098664b000557e49177ed607712bd2129ada59593bec2fe22114daac005f521585ccdc6c273bc8e2a94987729ea6334846e56a13a3f0752d04eb6" },
                { "tr", "ca65b645e8dedfca37566870e375d9993e4eaf188faa9eeb3f11cb6057037f2ada30820af93238cfc0196f9f639b856a8a40f4dcdb013714f0ed43251e90da54" },
                { "trs", "3ee61f1ff4f5573f6ba2d5d282ddaa7d3466bc439f2871ac87d7524211a97d4164140f7d8d2397f6c4267b3c2e59627ec26f8db9258c69059cdfe260ea4c14d6" },
                { "uk", "a41a59d0279162df1a3c4415696cc53fd0ad78de92ce7a3cd3485312aa0459b8b1dd4bbec2b19319947e50da589c29b00f27ea5fa7952688a7b58b0ce31f5bb0" },
                { "ur", "356c3b9b994e207b15dbd782f4a7d3693e82eb4444b948c02544a95e60009169bce7c1cc5eaa960808cb4c21b397d02c329f8b19ed9d95c2b6378a7f2c537aae" },
                { "uz", "fe75f97d812a28f814ce533e64a1d8832129a23de902e82538cd998ac244c6958c78d6e8912b022aa9617f79e9df2bccfb4a78b469ff6fcd585531c104fb4219" },
                { "vi", "2500b021dc400db8614cdbd2419abefeba213540ae9ed8367cd3101ef5b14cc9c1567644eda6df431bc82519e4c30554f8883d92195434c16cb63c4d1ca1a013" },
                { "xh", "686ab934efdeee43dc56fa335628efb1a9add128cd28526eabc2cfe4e9d53c04e666b7c30cd104de915648c63b594d9dee94efbe4bf397208e593be76c0ec275" },
                { "zh-CN", "a43ab48f4e3ccd5953adbc240d0dd3af13e2f50040580aafe99225fac2e4448033f1c9b6290f260420c76e733ce5d9bd7afce6ab93b02316e1183147a36f6019" },
                { "zh-TW", "a4391fcc5c11b5cacc67c0ba0219b88131fa81daef231030924b8afbeb71275ea9abc03bd8c568c077162909d1a3e02e10311bdf9fd6c43407313e1a602d6811" }
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
