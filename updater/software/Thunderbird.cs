﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
using System.Net.Http;
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
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.0.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "8626cf94d6d7158e675addc7d4d1c90c896a6136a88885993fd88364f7795ca79f8f7f8b5b578ce7b54f099ee484d8f505d644ce8e3f0c5d7a7ec3f1fb4ab888" },
                { "ar", "0cd1731ccad6579cb58fcd9450b062f79546027fa01da2f2612659efd9421811c0d4a9bc4fbddc27c487a7b73208c415bff810a4c1753ab6f37d4f8340b2bac5" },
                { "ast", "6f6c5ab5de3b5f304bb1395da20481864a901d8160e7c1de225282e9a515a186be4cd0da47073eaa39f3ae2422b8a31863213caecea85b6fc6fd1e07c444a785" },
                { "be", "0d5e7fa7aa65e11fa2c46a411e4dcce477aa8a089a8a8329302bc495d8d63bc4c4f189fa9962ecac441eaaf3976620800a2de0b88cf1f566b93d2d5aa66c12cc" },
                { "bg", "1b52ba5f53e14e0701bbfa45502299a6235d40d8b7552d6bbdec97afe6219695ebb59ab6507af5a0dc0cb0ce422e63c466fd85ecc231c301ef715a75ea79681f" },
                { "br", "c6c07c627d636b872f3babec671ef74c5435cd8d02e0464b7c23f0faa890bea5853e81a79df102eaae72bfdacefa1a641bc7cd9d65902394727760b39e77d4b4" },
                { "ca", "dfd231410ac09c36c95846d43058364db9519efc48ec5f8ab1d26f30d28a3f3702cb94e92f0369ce01a1d3ebcba7631cb0888f37197e47a75fb5f27c83ee8aaf" },
                { "cak", "457dbe36577ec7e50066e0b23652f8814f06091905da20d5c4bae8aea8380a0fef7d7e9838e8796b1cc14ec62c4810e8a9e53cbdf5ed8eabaa6941b82d4be942" },
                { "cs", "9a5653287743451c23becfa407217f792ed39245530c99b4afe8faf11572e07b17b87982b1aeb4c020c9d681e609ca2400d97be5032670b43e8116052a658709" },
                { "cy", "81fee58875f43db9837d98a5719067a0af9bb7492271879644c14cd3b61e9c87905e33f70bdd925ea844b5a0dd9d8eb7284d597114b52258e6be1b5fbc55423f" },
                { "da", "e07d2c10aeb73ff76e23c23ec01b69f1a57464b8b6a5308430f5d1961f1de6e69c9e4b5ac323a67d4542a974cf1c8d6067935d1f3dede4f4b92a6af62758e0a1" },
                { "de", "b83a59c2d12246212f5fa2dc652d95cd99ecb54f20ea13bc2760c8bf2597490753d1bf8f3e28113bc2548e2e9c41ca1c37dab982dfea158ee4a00ceb00af3818" },
                { "dsb", "2520b75dbc7566d810a51e36c44c9b70340088e5073dacafa3c11b9d0446e7a6914c655a21254f27a352ce22d67ccf468763b8d0655ec580f28900d3c77a2eb6" },
                { "el", "147d349d2c53a3f94e3247607f144325db698474be15aca70025daa2e57e29d37e2c9e68581dcf8afa9985a368b15232862ec831a114ce4f35e900cee24c820c" },
                { "en-CA", "1d675027b56e9e69e7c5cb9c6064df354fa0d8c756abc1ef5d7364469d2bff8ece27f29ee6a69c36f3cd3aae5de378f12ff42949fb5f4b3191f6015949f9e6d5" },
                { "en-GB", "e7839e193595adcd64cae7f86e6010af1026459d29c188418a6822d0297dcfdc7e0297799af51bc985cf9f19033e87d69b5401728916efe3e3cf667ca8fc880b" },
                { "en-US", "8909cd05471cb3ce287e9d8027f36427e821f344b2dfd5773b5b246e347472a708ae1981f4c31a9c61e07170859d0839cc01795ec427ec41640398d0df50f38e" },
                { "es-AR", "2d7451ac67d432fbb6610648b76de5fd2b078e3bfd3da8701e1b74c1ab14314e7d781bf8e115e45fe10274e33140172bd5dc5e55b698b93e8110ebeabc37e1df" },
                { "es-ES", "3b64f84c4b5316d373209b2c0ddda756450e043f3c7293182802baac9bb1c2d249ea0ba80f8e4d88a3b8d651db66f7ceea94fb05e11b05bdf5553d3d7a1ac59d" },
                { "es-MX", "01eec8102ca5afa8fbf4602d86a2eef54dc480319633f1d61b7d917ba2df706c182081f2b57123fe70702f34b0ea94b20d1dfb6718e91fa7518f6559ce96a399" },
                { "et", "bf43daef0120c8ecbc02fd36a7da0f24c3b6983c241c4b50b57a59d783d2fbfa1069f88eaaf1f5b409c9ff5e9892c59fcfa39b05268ada737b06148dea53167c" },
                { "eu", "d6d1cd43c43caf1aaf7d421ce2ec85eed959d73ac845a1422606388680de16a5ae1e11ae0d8a97442b38df6e23f8f173e72832d3807678697745e3ef18acb881" },
                { "fi", "a03b5163efefb4d95394b11e1aeab63b50c15490153b53e7e221068e7506e7eb509223cb645d06e5e70a4c40d8e31c313f3b8acad7f95d1ec996c4d43a3d05dc" },
                { "fr", "df8738bc9ba5439e470145f2874bf347da9aef9c566c0a5d49795586f5847a63918cfa2e2173d814babb9dbf9243314155a7861c6dae74582351de56d06b9476" },
                { "fy-NL", "7067afbc4ad74e9a83fb69528fbbfd9cb24489167cce6ad87cac9a780c71c8df2d7770106edd543c284963cded624f299e1c4d41c32ca8d233223a9bd0a0745d" },
                { "ga-IE", "444a02cd9f1cc1f88d4bdc55847572811bf7109916f8a0625d3b7757895856cf30b9c834271bf5f81630c93b90f4fbcd57ba62b8c37ee29cd1e8d021493e1c20" },
                { "gd", "975f9b6caef7500b7b1a96b187f6bba9a6305ce15e082efb1193db2dd494ac2b718532f463a9ecf09575cb9d465612060ff9c180821d4f873b7988e59db674da" },
                { "gl", "17ff262d2b3280b4bde8ee04242553287caa6b1cd68bcc6b2c108e77f4faef1d253a020f67570b7055f385dff5c595a0ff7721b2262a80169035e0b2c3a20297" },
                { "he", "c6b916aee1bf8c9b892653b51c599c37657e3d3c034ad9b39d2616df9be41ec30d29870203f8bc9d68b827957026edfeea21bc193d03c8332d7de391a1ed3647" },
                { "hr", "00f496515f770c3879bf7890e4eec4359c484ad6e37b4e4d26652de2771376b583e77a996fad9a01d311e22e960060654a4cd192250e40e874a680421e84fcbb" },
                { "hsb", "d7df8364714c3033355b2dd88d056b392c1872f93e4682c74ec321861db4e7399770fbd1b4bf65e6b716bdf21da8abde11c0fc8dc969b469b3d281d59a215d63" },
                { "hu", "fa3a4b3619e373bd0ed72392947f919048a48b44963d6b49f66cb864544de645c4caf1b4f8da43d1ee6884cd0b648bcca3cbcfc331330297a9d47649a7477294" },
                { "hy-AM", "9f301a361237eb90f525674feac2c518da14ff7cc57df3619ecca1ea267a76757f5746403c2fb0232052665f35f98ac8da28e6ae6e53a64b0a5cd689ee58f296" },
                { "id", "5c4d2cac52df9ca58b5fdbc9f7d2dc4f68b9a32acb48a8cbeee3b73a3ea1215d5e50f5bf6aef0e48561b85cea4e47b28c5b0407827b7808f5337516309fa846b" },
                { "is", "c6ce4c71742145abb9b10a00f7085b2b4df4e580a76ac7ef9302d0763b0bcfe1ea4febbe30a09d51bdbdac90ed065763f5abf74df902e2f5ecf281e94921fc31" },
                { "it", "3294e0d49534e27685dd3ceb0222e9f884ee3154d9c1fe34644b239f1045dc3d19b2ab251db535b3000dd835f762e5a2f970c86deb1a9f5509bdc381383c0141" },
                { "ja", "66fda547fbdcad176bc3706bce1b9b708ce9c3ee9d9224c162f276a87b5e364e09cef584dd345fcbcf5eb0e714763d7f0cafaef9318fe0f431df778d033b6da2" },
                { "ka", "837879da3e09a9fd6d0bfe1bad0068414e6c6d9dc21bcdabd8e04c7c97be5c9f5525c2ca02547dd280d6966084c3be05efa9b8c10e961e72a6bf7139727ff625" },
                { "kab", "04db0429990d7a9ff77870ba88025e30b462b9a55585e1c1b6e893cae0fa38db700e9cf39e0a30e94d2745f608f354ad4a2babf341fe0986cf926e017edb42e5" },
                { "kk", "cb3983ab4d813a8741a266a4729cbf2f101b0c20692af74a65eaa85e980191d37a52c1361d6914ff671b57470ab8df2669609981c0894b3d7499196cc8068b67" },
                { "ko", "a72f94d72c8faaba806c07abda5137b7f6a98fbb17fc448e4ab7451e4852b1d2601bff92e070bffeeae06b0d1c145373a161371442270655660bee27e136e5bf" },
                { "lt", "ef45c05ac2ddd8169d8f1aaebeea927b3d0a107c1adc9bf7f8936cd97f25fff77108265bfb9f143af67c08852831609bea39ec77161c2af82d06e1a95ed65663" },
                { "lv", "23da6f81a80efffa15564552c5b499e5410a04b04cb884df3601a3d306ba8e9ce747ae5b577ac49ea9881135124d5c5c5b75342b3e1b0eb3f57e3b647762ab8a" },
                { "ms", "3b5f8a72307d6aad8c6db6ca936e0577b23dbbc1750f344791032ad55acfcf89dd5e85a7237aa5ac2b94de51f6b5e96d8cba9f78bacbee90ebd846587fecd44f" },
                { "nb-NO", "0f381eaaf7ba1bee7db7984cb5aa996f526a4b9bacad02207fe4201ecccf0626480eb56fc5357e6789ba55bc9f0f63b58d052f5ad5a55930611129a806275ef3" },
                { "nl", "9625a67cf98c276e7ac7da7e9edbf26adfc41dfd4c48f5d002658c27a821c53c4fb603940601f94f8fffb2cadeeee7d590fd78780b69b6687eed3d5314cd2903" },
                { "nn-NO", "3c1fe500200593410cf5f65d3aec4467caf91629d273771b38c3c8ad5fdcfe39cb3587a881d4cb0d1b71f0efc14fc7dcbabe76a408899ef6320ed05825848a88" },
                { "pa-IN", "b06c6683366e344031ebe10a16b813906008759fe9007f75998458961c690d9c60af6cfcd42c80e6f13b674c80a3b2965b24ec95bf25a6f4d86c1f67bbdc0580" },
                { "pl", "c98d6e41cc46b2228b5916d6096bc43f6a8fbaa0a17a8bf4b1ddd4395a4d661eb388e79a2bc4eb8caf0dd11b0d39362451ee63ffb26f63e61a23b7480765f91d" },
                { "pt-BR", "9efaf3bb3d1e10b3618e257a90c133803c5e27f606b2dcea3be48dcd5b447a7409a675765b1eb518db096081ac787edd09218f47efd74bd1a9510e937b3e454f" },
                { "pt-PT", "18e66d168aad4f08f484a0aa52af33a9f796ac3593c90798d15920fa74ccc4349847f0fa4e032e6862db72a81c90b2df53c8f2e03152a5d193b7025e5aef9b78" },
                { "rm", "6beec3d5b5ec618014a0952ef9812792fa205e1fa3652c73a8ac22df63cff483f71438e850b56eaa8625acb2e8bfc256c3d1c1ff8fb82d8bddab980d993d0887" },
                { "ro", "25cc686d4b4d87f0198ebef8218ba2c8f8d8d14d8fd383796f2c9243f66d80c889fb4ce4cfa3332944086fc875ff222a48a6d2f10946a487a5d979e919af6d5b" },
                { "ru", "92505e4fae7685ee7d1b5d69b85722634564b3ccc2b6292bb5135d34150b315dacfdbd64384ce5dd39f107e15e5a4d677f0146fd7142a0c960f68f2a48a14d5c" },
                { "sk", "9b8a280e8317b14852bb18b980d585381524c55caab7913d9b492a7128f808d084d9802e83439a813fbb9c190881ef6ca09ca8c8994c4c46a161a6111f44df84" },
                { "sl", "f304ebc4d08acb3836667a41c7416ee715e54a649bd9eeaa31a473c927f5844d4319ce2e7a20dce1428905ac7eeda0349b54200c7868ad12fd0071034120e719" },
                { "sq", "45d836f5455ef0409cace54dee77109a5369abab479e1719ec03e0488a3e74e440c1e083d1713e62a8b08d0cb0e974a269c401d808d8abc433155ffac8a0bf4f" },
                { "sr", "e996329056cd1b0be96e487065517fb3369567c28c2cb6b08e14f587f03afb621dcce7c6fa9b439c1946583bd34d212f5051c424528c8a8aedf2e419e91d8395" },
                { "sv-SE", "348ea13bd2547bbffd5d33b9d4900962cedcdd1a70670f1a5fd91d8b51a7b59fddf532315828bb2af40632d1a5a938d304747bedb2f937913ebfaa553dbd38fe" },
                { "th", "2ab10eb78cebc992781a79b0f30fa32d57aa8989670f0be0c5f8e873005ec2afc86e1bf12e1d8fba9afb40f4eff9a28b6c6edd45a64008b254269194cd7aac27" },
                { "tr", "9020390b9af47fa77beef0c70e1f16a1c35aeb2d3b450aecb4dac6334a36edfd18b5ea2f3ffd2325c6a384b18fc1d464d264677c08a018e6f5a259ab4ab00bdf" },
                { "uk", "b20733b7d2e933c6bffc2a7bb3c1d88331dbc904138bc322ce1c5132ae0d01ea5f6f3b129b6308ac5d0867bd7a9cd47c2344ffe3fa67f4e4022e6234388db17c" },
                { "uz", "4323af1f77d2e941663effe2a72b75676a6c6dcf1580c69c1009a43bd51f48411045dd0cddf3103f40ea28ce78a90ac6ec5c0489d715aa0217124a16506ba3af" },
                { "vi", "0e76b56ab4e150e5e55fa9cbe5f1eef627bf9e1b3c1dddd91b8ee7cb347eab329593f364a6a0cfaf7221f6c15f2e463ebd7ead1f89ecddb2795e107d974842c8" },
                { "zh-CN", "270226c776b85cc03a43fe9ad87e2ccb048d6afc341ed3f4b2b949b082a5343545102341d58549292f2406d3bd9f3715cb59077bd2ca404dfc3e94bd70d587ee" },
                { "zh-TW", "c5612bcf291ce1cdb706b85864a1f58638638d4e3b7acfdbbe3bf9a52843e7746cf7b7c927eb5d995affa4a7c584c3ec415535ff1c6ee56a590177d093da571b" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/115.0.1/SHA512SUMS
            return new Dictionary<string, string>(66)
            {
                { "af", "e9261721cb3853821d3a0041ceaf9d0b145983349cc8b407c831c4fe93cb86193639e063fe68d6016b7da138c733ad40f3ae3bbe7b969da78cdec837803774aa" },
                { "ar", "377d0f389b334481b026b7f3ec7dc0ac89e4131ce1155910d8b4c92699d5446722b5f6bf0497ae5e4485df5b889d6b8fdb932a842fa1c9247e91d6b1d7108d8f" },
                { "ast", "92dbad2fd16472dc8fb25a72bb46d02cc14237afaa30fb41ea420a0cb1bf6a565fb49f8ebdd2ca2dd3779c845a325ea659bf42963b3f6b5771ee05b6949ae540" },
                { "be", "529f164607c8db6a31004317bee42cab574141e9e1db8098f7030c986757d7ad9fce37d5ed3a6fd219e1bba931f7a4e51cf5a0d3e418c33cf117be62295ca071" },
                { "bg", "0a5a7980ebed71c0e8419df68c3544803bd43157d9bd9601a7fe9af310b9278ff40011ebc39eb6d05951a901e2387a82130e6de733fd2b8b512276e8f62328e7" },
                { "br", "456e563596a46730208c4245510fe146b70d9cdfb5cbb09b8bb82f1435cac841e73ab981a5a717c7f12005fa46591535513ebf450c5d4e2425a269560c75c759" },
                { "ca", "e0c0168b14b5dbcdd97c6e7d86150a10841246f3a1ec2fcc7208663367724c2738777251f9306bb3ecbef73dabd3b7d9a1cd8739431d2ccf4d26900f108f07de" },
                { "cak", "6e0fcbfa9b00ec7931f8938ad1e83db64a59db6d682bc96de01ef296b7df677e29e0125ee7c57da7814234d0a50b61c6b32de4e7651c525a8a4086a3d0f488d1" },
                { "cs", "57b6d5d7ee6cae23b257d0e48e53c607df4577974335c7deb7562652d8d37871864d065fe93dedb76f4a1440c8749bc804c7b10c6ed74e2882f5ce4566d86c22" },
                { "cy", "311cd330876901f8b9a0c68db10d8bb5a0e45494761b89004d3a4642b5e89d4395af5dfd53cca915baf5e0c486ceea5ace46531905f6ecdb84c5e7dbc770f7f8" },
                { "da", "44d8a7cc6c56dac03e72611ef7698afc26e14c7a2dd78ff566b5d7a2260f9a47edf04d0841c9557c0a3c6cb446e12c4f6b2a8f6750d8ca43262aaa987d4be22e" },
                { "de", "2643898babcf180e94dfb0b740927471383060c184c41833565259d03c358ec4e735f3a7670bf98f901b7f6056324c4fe1e7ea9c6e654c4a279b0fe67680b8a9" },
                { "dsb", "8c567474dfeb9673ddfcd2ce1d74cc57b98ee8902af75964b9e504abb20ed18ba3b6a95449f3e13ff1508c213d1d853a70a909a7243d9146ce4db8281402c5eb" },
                { "el", "41c14911fa3cb36be6b2dc61bf3e07a2346a6e968273631228434404c980d036bda0d6587c32a7c7581ecbe65afa42647b041bb34c2d282900db1d85cb4254ea" },
                { "en-CA", "7a31d5d07d27b3b5d344bc33e187e762c77311fe2f0b33679a5ec573e52415c6d70f5b4047b7042875ba85bb0354bf06920c0cb826a0da55c25ada45e5a3b9db" },
                { "en-GB", "4e2688a62fe430edf18c7d00d6b3a40b58317b21a6a048646e71de126c6722688d8539fdd10bdcb2c687126611319153e9bd161169569cca2650c31da0565b04" },
                { "en-US", "55c0df72a8b2b8672129190130a5678e6a022d7d915f106e4d70da9bd50661dd55d32982d8ff81f7692e5aba5e1e686a02272ab3b213b2b2eead3fea5d978454" },
                { "es-AR", "814416ed26b8bb955f890c22d1dfe6c4b5c7d9abe13e211a8782503943e643db3ca57869ebbf816d241c70f1920ea770d70ecd102bc74a86741dd6fb03a6e2f9" },
                { "es-ES", "18862454a4828ea5b662d33b406b7cbfd92fd10dd50d4003c096a55c5ff870f2794ef8d68da442e6544acbcf5d7d86d1dcbf95a3f238a5465029f5941f5bec30" },
                { "es-MX", "a9be67c1cfdf1cfb1ea67121ba9fe6b2223eaad87f816b8519ab3ffd499a76222fe0600c0f2d64aee8e9e14632936fba94e15a72e206ace8cc493cb17972d550" },
                { "et", "687c8e82538dedbbf57d98d7e4734b6d64438a625adc79d07b382591f07d3c969d6910903f362545ee036c7372c096557b510d54e603d8f9bce3bca38da0aea0" },
                { "eu", "215269fae44f9e66f69fabe2d7352c65883fc532e5eecc3f2efc4dc776cca916260f0e6ad8930cde2ff44eb620cef1bd5c54c6306c3168647290d60da79914cb" },
                { "fi", "3747e3949fd0884f2a581f96578adb1ecd5ba1341285885feb099fb548483cce2ef3ca5daee9d16f7d8970b5b6d42b4fbb39d836c8649d77dbae50090b62e81b" },
                { "fr", "dbf8c097cfa6e341c3c0ff683f80a3ad5db0d199c6c105da5f6d55048d8c037e35810b57270b50c8fd0f299a87dbdabf761ffeca28ef2eb27f84d1513f0102a9" },
                { "fy-NL", "36668806de154bfb1763c68c201c386a6771bbb03c9d1a6f15dc13ce8de423c95623ea7eb56e07e9b93537bce9e9e58c13d24316fc6c9b1d5afe38684953fdca" },
                { "ga-IE", "4a3d273fa8d2d305b1c64e9dd85f44a82086bd536385f12726f30c9721fc3d6cec6c0541995ffde419a8d556168b119e617b3509f6dec5c64b805c8e4361cce0" },
                { "gd", "4766c23172898880cf47333b70de2fea8d432e702cac2b5c9173f1947852310ede5ef2bff1dbb94e0006738c1ddfe62689eb0da9748c5d4044f22ff3ae65330b" },
                { "gl", "f52d9a88a87113064cbc80daa8b3aae08f955d4ef02d6d24d1b02b4d10f6909b5bf735348ba9fe924ee79b5d815dd527135db93b536091ba14f25a214342328b" },
                { "he", "69437e7d2399b22cb345b17e1ae008d19ca4ebf3958037e86a506fe3bc7b24e33cbd9420ed9cfbc545fbe3e7006bfb1a4cff77cb35ac44962620b81742415a17" },
                { "hr", "888ccecb4d64f52bf368ed38e80e82fda5a8bfdd13ecd2c4e5018432dcf4223a3b83446d6057a33392050958add27bc73989b4fcec53d3fa5bc9c15b24c09ab6" },
                { "hsb", "8e13dca245dc8b172465803f9964e3f293d2ccfe321ecd751ee7b39bb6e8335a1e03ab1596ef86cbd5c02c60c2467878ce88bcbad666b564dde95ed0e96395cf" },
                { "hu", "127804ecc5810d461c92bb1ce29f9589ded0746e6967fcd50816ea86ac88c40abd0653a7dcf1e72584ec4e13e24cd7377690abcc39efa3fb75e25e80bf2aa350" },
                { "hy-AM", "0eeae5ee89d13836b8a1f02982d11d072be93deea12b2080c89268bf62be70d87561c90e8be91b037cd0922bdaab298662a9819ffeba33532d21e11464c3e21f" },
                { "id", "ad37b4bbc95b3c10a4ab00c6fb930ca42baf5ec57577555ab39fed29c6dc39cc1a13472f31876bd6cab828afea5a622baa4a9df5bffc9d58e9701e19a84ce097" },
                { "is", "445fe33b60a33610e51241b721fe76268fce0f92635001a164978356fef920981b94dac53c097f033e561375fe8eadeb7ea62691333da686269b563639275ee3" },
                { "it", "299b57808395357e25bd100816a9dd6ff67fa411c45189b38a01c2c4890af0362998a8809edc35990f31a31803f9b9f5d8ff37ba2793e58339ad8b1b71844fe4" },
                { "ja", "2260ab0b7006a1c80c6b20c6831eb46a2b9c03ec6c0a862c041cb4eadedcbfd891433630110c0c32daa3e12839cb493599731d8385f99c32dc1aa262630afc02" },
                { "ka", "29051433b530a33080e1c92b7386a73de3de51f26011015350e8a4650e54b7b05e6c7fad88d9aff19e55e92ffbe24838f61f9c95a3ea5d2efee5f201e132ea09" },
                { "kab", "d397d0c1d9f1c792400075a29eb7271b688ba5192359f2967f1068f3b712d1d91500aedddaacf7e161fb21d0cfe2a0ded7254acbf19b5f64d6061fdc0227f8be" },
                { "kk", "b582a279274b447fa7c2a7adce5eca15619479a99732e2b7118b590dd2aa7e4fc8621d5ec995348ca76b3081a64f06db0c97e59e8bc26e7a50554b5963554d81" },
                { "ko", "10c5a106bbd18fe9f1a0ae9792ffe6dee07b8b1c5ba346c590100205307b74c65cee1754347bb429bcfe3a0ed17b5f475d80bd547ce6b42745d533f88c9a0193" },
                { "lt", "80e822128c9e80bdc5d7918c802c23ec221f46d97b135f2592601f7f2ce3e4037b2f9c0e854606507d19e43518184483223dacf3d43ae7312994413cfa584c91" },
                { "lv", "350088c44f4d817ae1cf5518cf957253f1b0b8431be9a521b8bccc7e0f8481463743e7a78629c9f56c7a40bebc5d739e95b1e8277ce1522a750132fc95f5412f" },
                { "ms", "f767602480995460c3078eb3b41fbb1e86af6924e37fe7a77d044fdeaf22b3c5b97484409bf96c844b8c05437cd39f0e7636eab7bcbdc03cf60cb06baa112aa0" },
                { "nb-NO", "33da32000e5520131a4fd0be205752798087acc0eac3123c0df8ed51c7d7bbd4cec28372c7ee4fa393825b6d8ec05cfa09f1e23cf36402227928b57113338e9d" },
                { "nl", "a0d86146a20c123cd3556a3b7a3f77c5219825e952542864a39a4596a5593b085eea1dc3063ffe223712a614d87da13c1afcb379dbc337faed70c9a38642e525" },
                { "nn-NO", "e5800e795292fdc06e015525a1dc451436a0bbeb02556e0fe85e22f43a79e644558170917dd75ad32bcc3a63d0fb69a9cd8a3b68056788d0ee332c5a93140214" },
                { "pa-IN", "841e7d9445c95540a575eef74880441b4db7f3ce84bf41230735a2bb7695a79ca77a917b55a06bad720c936e4844d55bdac728873030ad5ca267c732809ea658" },
                { "pl", "2c825ce9a41527ab73597fab4e52010792c7c64a07c44ad4713de65e33ee6fc4a1fdbf87f3eb9ed6268a8deb0a5907f457ffbb459b882bde25a607985ff7f88d" },
                { "pt-BR", "cd2efeb99484d568a55a009ce806c57a477827be894c5c81332ce0f04ac4fd4dc4c10ef024f4dc522fa8cb72b944541e0285254438871dc0b21f1450e53b46d8" },
                { "pt-PT", "fcceaa7377d3d8f6c33946d50b50bdf54dbb0e52b56f98a20a285cd44c0ea1a81e3aa9b65682ee47123daa645d4d2f66e2c43415ada6c82e0f1c4c4f17377940" },
                { "rm", "68ca7b02cd7a293b660302e0e87c12a91912b5d21a6aeab931b44807851362d862a0eebe3a9a64dbca47dd2c498b2cb26219fb316d93f5bb6a631238dff1a7f9" },
                { "ro", "c613c465622b9c20bb14857174c616fca627d585c2b7712d3b9b95843f1f9e1970ccbfac53819765a2158a35ce5886f0681f712f5631fb9780aa31223a2dfe2a" },
                { "ru", "803f0d1e80ab131faf29b52ddfd70cea369d1d5ca3b64e79986d932b29d0098e43b172eb68e2176b7738148ce2c734e1efa95ef9535838e4462a4285e2714b96" },
                { "sk", "2ef2a830fde33e0070b43c29a07e2353ee866a07633a0b2773457ed1d6396eda86716626a19271acb312dea1dade92f1876f721daf62cb84a58e50bc749efabb" },
                { "sl", "9bdbacbac604ffbc47830903a203354b67655df8e1dd012348240240126e10b85a6208a1dfe0dbf25859d8e53544566ad53622d1a7ccf28526e920ad61de1d61" },
                { "sq", "2cf3c0343470acfbe926d1a396aec7ea4d6197570aa6ef8984a7f450f0eb8b430e3cae5ff29fd5f56c43a1c2ca1a9ca9d470ad13e98d2b75d755dadb54244c14" },
                { "sr", "035fcd9a3e748c8bc2a5cf8ed6abb723effe4d1c6571618e259cd9cb2e01d77cdfaf29b83fb99a0f8e7322edf2d125770c74f1a3d1b20fcaeb18328083f49b6f" },
                { "sv-SE", "0047c7252067d1531b5db889248e90f84d9a03df1e2c0321f331eb9612d79b29ae51eea1b7526c9c9dc9f62ccc75af85a60a6668d557034fa2674578c4540c12" },
                { "th", "24d1da01f7603b510ec32622ee1e7f1226d16dfe0f64edb6ea48923d6eab5fbf7dedf67b821d8a730f92859c38b247df1bcd86e746a4e09afa984be9f23c6f50" },
                { "tr", "15c97f5d654ceb192c61f04b3d00df08451531c06846a638789198c09fdc48034da90737c75b56d080fe7c7e141f3bb739a095af242e8779179d1006845195cb" },
                { "uk", "eb726dbf68e274e9d4891fcfe46648c58b77f79461e162fbc5e55c442f76c025c949cdd9552923019ff9c0d4f472849cc005506bae52eb883d05fc82c4d0a626" },
                { "uz", "ec2d48c421256d5a917189726c31d1f0f5e4855d22add2e5a67c99ee757e1f27e239c04e0d95fce1d93c84f80beed276ea0520811de1d12d3560277dbd508f42" },
                { "vi", "051d93265838b090874c41a6e41c1ec065c8bba8ad8320801a1580939a68e4b9cccc4cf3de94f878a9a293e39ade6df6bf30bfcced171333cbfd257417269258" },
                { "zh-CN", "ca43ecb21956ec7f9cb791b9a36d3b95fc2a8bb401d49ef9cdb6dbccbc978326740281cb607749d14e5a6217f7c2925bd4383e5dc5444e6fd038691ec58df105" },
                { "zh-TW", "d3eb1bd879fdd2768038fd2dbbf24bdb459cdbd9997b053b7f2c6cc8417de9bbe4a21010deaa9c81f3745e3a66b7def9496b10887440095bebf27016acdf8e33" }
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
            const string version = "115.0.1";
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
                task = null;
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
