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
        private const string currentVersion = "115.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b9/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "b94de93dfe4babea1b6a6e2e24361a54c28bc8a223b84d9353c137309d426b0e51b60c5ff6c417e6bed6a3f6e7f8145a2dc2c0b9622fb2f9145a1b480355d7d8" },
                { "af", "c0b03ee0d651d9d376d50d455741dc5368aa12f99eea645602518d4f635a7dee651e156b6897a9a0b5ff7835fcb94334523b7874fd29071f1b4b65589b5d9563" },
                { "an", "474bf55ebccbe4fbf01bd1894afb12e8ec953b55289f8a2b9a5ceea1635121e13721f85283cf2f0620311307baede6bffe000a3d40c02c64858bec8e92a34485" },
                { "ar", "e2842000a4f9a1e182483b028bd63905daaf5ba855f0a5b955a637275dcd2abfa6feee886782b5ce6719486ad1fc3cc4b9da97e43ae94caaf577c0af2762133a" },
                { "ast", "e72775643378f1349bb9374c1877c38dc4d19f7426ae94e4baf325ea306199c4c470dadd671ae33a76beaed50beaab5bed106408a518f54cba369b0efe9d607f" },
                { "az", "3b80023855d49b8578b3ad82bcbeb5a0dbda84239cd69330344a6ab585504c3ced3b97881ab05f5b15335afb599067205462fed3048574fe38ee3d453b6f984d" },
                { "be", "9aa323c29089f0d95d7e7ec82bd702945ca2cf7cc362cef715afde2e68950f4c61b6b0a71f946514bb4a9085a417c07d6bbcb31222c659c7784c2f8a6699b9c9" },
                { "bg", "8b6b1f74d301d637740d80e46e901e0483519b84f274b4293e4d5266a256f1e786b20ca428570d3cb04bc58496d21a8dcc38c8f2928fb831dc0d2be62294bff3" },
                { "bn", "653cdde2773c9728a0b062c135433f0e1401b3f0558c3c206c497c723ce0883c816dcd7d5c5e598d2a6503b13ae820d2d57392511b41ccfe1635d6c9f3611751" },
                { "br", "5baf22bb1497ef68975eacffc812a8a7d12c96baff997bccd49f4fbb274ea9436f486d645d7e6cc5af2d6477df6b847eae3cc4e09735eaf3ac0658acb09a7329" },
                { "bs", "55510f8007aa7a80cc49dd6bbc33d714c772156e38c2a5591370cc33f2f5c432d8dc9b5ce89c3c56f2eb158e16cb052583b6295807dc2ae50ce8bd7539ca32ab" },
                { "ca", "bf5e5639a32688855d9a55f0337cceac0c5efb502161d8860978fc000e5e00ddaffc22f132cdd566464596743f3d9a2dee2f49911e4973917c0cb34f48f5bcd8" },
                { "cak", "ef31358237507253ef7e5a88e820115c81beb712009ff032ad9fd86a20bb5c6a35b804c405faa2046d75a28c3355455eb157dc441d702de5a3d1984252933384" },
                { "cs", "b90ff92880f09db363dd07da720e001aace007d0c0f23fef24e8f93509b470b2857a02593cfb0e6d36702ee95fe598a656f01aabe2f9079e2fe81b1b3f599df3" },
                { "cy", "60dfe4ef880ed2b1558776b13f33464e3b94d859311db4b2839b29ee4c0fd1bc0add432a53bc1ce01b7ae707bbdede14e08ac3d1958f31e1d786a8fbd1433bf7" },
                { "da", "ab6015f2d4cbc5ea871d17cdd051d1c1b1fdb009d4fcc11aa8af20fbbec16ab0569af200dda8d70ac827411574e99ebc380f744b232b1981274acb201e7c9574" },
                { "de", "87fd1dd2d46cd94f556ea825a63d1ecd7f651e65395e9ffe695c0e89b4a5f338cb805931c52b2e6d35df6a48029f793b278802a3cb540141d42d03348fa3c735" },
                { "dsb", "c65cc5b72275e3c005db9763341060f30818602bbbd85fa05eba8f5132e721a33a4661ae50072c5a2d49ada9089e060d5ba3a7fb23b4e814d8fc06cfc7b1144f" },
                { "el", "af70422e4eafd14be2f5b943d44fb293c8a8063b5ce5764c475cdc7061ab869399cb6296a8c315e76346bc8df44979b2a0a54f4ce1d24d81a852a5104b411ee0" },
                { "en-CA", "59061f857d78b978ea533d62df7d738b290e1e34bffdea2a2be6a668b5e07a6c5593f03c209d83638d4e1922ca9f5e80cb31aeaf591d539ddb124ec72949abeb" },
                { "en-GB", "980c60f83bc8065cc0a372351edcdbf67077e7d755cd4d562cefe571bf62d041fe28af1df67981f2f7922a9f0ef8c1fbac686da842fcc9116e22378bd12c3feb" },
                { "en-US", "f7bf7736994b1dad976c44a38aba9b87a8d69dfad8016e294ff9dc0cbda20ab277cf6567612eaac2c90e8fd392c6fc8c82ed47ae0185e4b7ece515266df6b129" },
                { "eo", "a24e06822c2431bf85b328fddc5c48f4ca50a8f5d1f94c85eb9c8b54336c9d1725e0073eb12efa7aad085c3ac3bf550048a912123a2c9dee9a07eaf42b9fc164" },
                { "es-AR", "06377211c08823a541e8efd11996fb7e26cd459d6bb5b67b9352a76b1db2481719178d3444dd2de82ae28cb352fadd3b05009136492ccfaec4d004f18d2e80c5" },
                { "es-CL", "39a30386eb9d36c7bee2c7eb7546e2a6d97e77cfb0a8de8ce21299c0a25539b30085d22c9b559bb3429d41a60e83e644ea09668a758f4b598a3e3c4b17fabced" },
                { "es-ES", "3b190bade59d14421640c7d5126c07ebbd4c5d34b731f9d808fdff1134fea612dd4c25d0b9fc15b37775a2db897833027e3d0c690f2548325f6c90224cebd248" },
                { "es-MX", "bca166aaee676dad8286569e0d77776ee904bcc942ff9df9d70ff48c65edc8bec43e4e6871a22b914b1462ca246dc747d8a4319afdeec26cd1daaad4e35cced3" },
                { "et", "3844b19f6daf3c80318140b59af0b4ae3e2d81e5502bea584579fb4824570a79f8da24afb6b2c97195ddf87c68a07cbdc21c83c8eae0cf3c4cf10e58ac67b6d4" },
                { "eu", "b78f8a4e45f0b511d51f95e7a1ff94c0daef244d60f78650fe3a505d2e9070e3d0237c12d1fff7c02ab82d8b7a380b3f6e7369260dafaa5963b8074fd626da4d" },
                { "fa", "a278ee9dc0b9eec441188e6fae574631095cec037479962fa9518f104e8c5638bac8a2f944282693ea5ed520131aec86c415a3a280cbc870f41d1085b2d2e239" },
                { "ff", "90d6e583f4a45fdab1055796eacfc1f2621a0d754a1de668bd47ce6ea37f5dbf96c2f248efa0e0af5fdc9093be915ce2984c3b857a5a77b7441f5358094cab14" },
                { "fi", "a50a8237fd99e8a3a6caa0e284573b26f487ea4a3eb654b98887e82823f5a28de662e6f736ff89c48000f281e21cc19a00f635606a188822e6d39ae8b011202e" },
                { "fr", "4494cf610fef59486a7a4aa8b33d0b2a173d490a2f8c6aed026b90e7b96cd23d1d86f615c66ee7d5d54d20d516158e3e45ffaf72259110bbb5730d1cf2f3e8ac" },
                { "fur", "3e8f36d149761c790655b433d22d26ec937f49e93788dcb10b9112a65d666bbf510d59ddb4047b12c20300bc60e32156be5134c25b38ca4b459aa9bd5f70564a" },
                { "fy-NL", "e17e15f4e43c889b403d5f65b5d9a40d4376f157f183c30054c3001db7aa7e22ee6814958aa8cbf92c3b5e356fc8fce7ce223bf4c1c1b8d27728e0415d65b47a" },
                { "ga-IE", "203478738942126bd6678c5b53a859038b5589de306a80f1c381868ca3fe72b456e74a8a1ef6267b3b00462d712c10d4861ba45686fe4d732ce3e1edbe8c70a3" },
                { "gd", "942c2d40b96ceb0840ee105bc5cfc04d8b928b7d451ce16d6c6083dc7e5ef744d5010daa81c07fe39542e5d7a4a0d1952d21b23104881bef4a0599147eec06b3" },
                { "gl", "f946f01975f45703a9d3305568d8f59aed3fffd890dbc64539921b70385b2cf914f3f126264b100a53f6fc3b67a4b47d9be28e7e1fe549beadd8c4ac8c69753b" },
                { "gn", "ca43fcdcd6036f337adad0ed0bb2f4bff9859b5abc6a1dcda14ad29b8b7420dc7782fb8317c2a01b45d4ad54b6412b50a8ca711ab19d8924988141f4c83609e0" },
                { "gu-IN", "3e375eddf04f5a84f088d14d3282c047d4f55006ddb4bf099d2429df46b770a1368ac60db6d4d80d100f68fb9b480bf738e56fc38b09ed89da8a4fb59d758bf6" },
                { "he", "a3aff895d33ba93383ba6999bac14776a08abd38120e7e631fac9785d1ea4ca56e14748fb3a1ce654f49b5808f31fd59c041e19906b54d3654e1ef67d3518829" },
                { "hi-IN", "92f9babb5f3c3860a61d5026eec548cac1d2d8191c0f635939fd6d1c68f9dc97f21efe846341f9f1b347a85c0ba9c9fb788badb720bc39392e038a7a3223bcbf" },
                { "hr", "e33a2cc5574d684e96d3b04c24e465bd1f95fa093c20ee59a91824323566b8c2c86fffeb974def214b028ad34bf90e7663ddf0e7be7bf21bf12bee1053a06dc9" },
                { "hsb", "1acc7ef7dbce8b28c1ececc501746be9fcba08cc9e10bd6cdc264727c3daaaa6e9407ec93357b6bbaa7798361258573ce3fe46cfedb964213edb9faa0c8aa178" },
                { "hu", "c853846700aa2e015c4b059813145532ecdac336398e2a2d1f860fbbeafc83e938098496a6e2fa04c91e57ec961c5e54b7680d03aac799b102b58020472b78a7" },
                { "hy-AM", "e9358065e884e45d761c2ea07cebceb0022867af69e688c75dea63ac5c2cec9b610c6ce8eff53dc2f8423f1aa30281153db244f1fc85e28f296618ec1fb9d910" },
                { "ia", "ea745554619da974692388ac65901af98bf7aaeebf457a37be8ea5f9cdb652b2c0de17ff98b97b14a7d16f3916ab6fe10319e838fa98d79fa17cca165557f909" },
                { "id", "5c7dc6f8f76be931652d74452640cec7117a2496af93ab0773a5b4cf240dfbbfd16d1fa2f1e24aaa35a58b52ad43677f9f8753307164bec9d1af586e67ee7a86" },
                { "is", "d958a3897e977a9adc45798609909cea0ad56391b0edced12bb7a7b86cb1b6e92b9ff488bb12d9921dd7073b92f1ada3f3fb1cf4b5aa9a6358a6921224c92120" },
                { "it", "9e4428bf530141f53f788f07e422cd8a2bf2ff6e60452fd6561a1cf46e848918a1b5a71a9f117a5e461cf9b2684ee2984504648c5a2fd67be869d9d865c47c23" },
                { "ja", "45508f34c1c09845aaa9a619fd26d8daa49f69684918b8aa1e01a1ea5247b65d266196b48171039d4aa537d749475f4b95db4f096d966b4f5961695fd8f4a425" },
                { "ka", "3011b10150beb24f011b18ef3586f7aa2229440e507ceaf68a2d6b65a2d757d616bb89ff7148607d3d72e88c9c2901e60255b458a99ea63cc2fc53ffc06a213c" },
                { "kab", "3c3f6676e06e742386bef2e01be9af81d3709c91f163e5488cf9f125e395bbf5a8a2fd8f93e252d2e60c071fd0d8b0b032d3ef7c1637a018dd59a69ddc6a887d" },
                { "kk", "0a9f420cff65b3bbc2edc09e100a7c634f3d3d8efb2c3c981d8f91d6e049d935df64f637437d3c33a2387e54639691ebe1ad2ac854d314f7f8fffdd504e60151" },
                { "km", "6a09d0ba59e1c2acc96ecc9568aed5a35f61ec427e0f029f344f976fb338f163d23ceeb76644e593eee3ccb67d44c735504162e8e9d97075b1b85ecf1cc60bad" },
                { "kn", "16b93796ef7461ddc809817c4ed0bff2634690d04d2b9e4a721312a86b2d59294a25ee317ef0004300c2934f1768e424374754ab1b6498127865ed217414bcf5" },
                { "ko", "ae93012efc15163abde3b99dad05b0d808fbb195e296ec75f5705101079b7c0bc8def758df37f3488603edc751936214390798a1c9d49c5688c3e5c06c3999f5" },
                { "lij", "318ac71c07fac2f5e156d536cae413ebc5fb2c98fcabf189b57a2140b9a661dc8d2440efad95bbe8796f4f089d720a923d7678bb2f88ac4be8f17f63f69e2aa0" },
                { "lt", "048b9d02bfd8fb43f28071c8e2065ef7709c3df9ce5d892ea71b48f7a63219b7b64f8a91b474552e3ff0307040343bd90c58ef2e0defd2a6a9f8d9d56fdb1222" },
                { "lv", "d4393b1e4a6b60aadd4359335928166140f469b74c61c2af9b6f4eb56a270ab44b273c5fe729c8bb56ec70ab585b45c801cdd2514992859e40606c5e73edea26" },
                { "mk", "e244ea5b5b6a0c3e37e67ee9432b9aeb4a9135916ea63a767becb72f6bb9b94834b42c5843810718c0aef2fc5dfe3b8f68f55fa41bcae8621864e04394c634e2" },
                { "mr", "687bbc9b9643e5c5a674b31861fdea150864779d3890fb53692380aa6ff38758fd3b87b508b19701f17c4cbafd6a0826c27cd9a48be8df0dc3c692625504b8bf" },
                { "ms", "c18cd47d49b68262db59145bdac561289e87c780184249bf9e765feada1817be3c3073f22aa6db3967495bea2788fab71ae89e26d0f426ba5b2cd279c7078c02" },
                { "my", "94adac2a71e98fbb550176cc1853410f6cf4c7aca9c4b3d256b2ae3a211afdcb64a68c34305a6b2f55776217b2ab49230ee35ecc9e2d435dcd401f25352cd3fd" },
                { "nb-NO", "dcddde62599717557d54703af053834d30593281a3f10639344e96ba5b0d37a27b24324769a2da6244f7bdcd3d39d7c821f7042d80461c04a10508270aa1366e" },
                { "ne-NP", "b292815584bb87066e5c4543d74390703cd8eadde15315742c9c78472b3ffde277ab1efdc7f206b997ce79569bd8d8c20e225652649bfc0aebd70ce2746a45ae" },
                { "nl", "ab8216cb43f95d3d99e835a5bbfff61812ea1fedea311c2faac98916a04e8763b329d9d3ad50d4eb09ada50bae54181b1db89a554975fe01291436e8dcc19c0a" },
                { "nn-NO", "33b012d429b5a8a5ef2a7f5313b848ee6d4a0b88aa3fd8c439f64301391d1e0ea3e0c0c90c74b035cd315357dad3552a95722ad8095997bc8361b7f42172e6b6" },
                { "oc", "e69bb5fd720680248f461882db457aeefec21f5f8175c91404d7e731c6a6f7eeb4bf3655030bcee1e42c0e929354d44279d3edf97e5a1d3eee6fe590a869e680" },
                { "pa-IN", "e5e19651c519ee61602a4ac7bc4c4acc58a8843221044f38a1aa3c23078696bae25beaa769554758d360c83ed423d712940b0a1d883303c6afbba43586cba069" },
                { "pl", "b482b5adf20cee5aec9f275b9c867c20be1f42a6e31d40e87172cc7f9e756b1b38372303592a07dadfd68f7afb0863ebda417c7bba3601b16e19e22e6a63406e" },
                { "pt-BR", "61826ded16b34b802536ce8eae71dac83ff65b4bff5d05cc77e7e8b14bd66ef6b67d25ef69ecd97a5c3c8859568fd72ecd75c7b76618c5b9052c8c169412fe03" },
                { "pt-PT", "684063e3da64e2a120481bbab622c0764567dc37ab9ca2479508ccf85f466dc4239ab39c6dbccf447c2a36be3f6fd28a158d99a2b0e847593eafc2f23b92c9cb" },
                { "rm", "c92683d4d628995f16885c81291c9bd723af31b59ad96cad426b16c9edab8f07b7df9372264f2d54a49a3adf5d286cce6125bd04938cadf60e6336e71bfcedfd" },
                { "ro", "f4e1c48a2172bd422ba175c88b4cc6b12d7835fc0bf1c66fb5e7e1bd483b8e319944203e288161dd9434e86bd544d72da8e36cd02afd16d02369bc0321736e19" },
                { "ru", "6be9ce54f079636af60eb8a8e24b07169b36d12f5bc3325fea62f7a57de0b49081ece466c519f038bc8b3baca8b1638528ced8580028f4ebdab20fc0087af036" },
                { "sc", "2bb38aea359fd1432758d89cf704f4b63181c58154837836b9871d04fba2ec90bfa6c2001522056e80a20ee8be53e443a12a17575a414012fd029b8810a3fd50" },
                { "sco", "906546a3185977657b8142ed8730a725f5c2de7b1857b110401d330225519a05674106516de2f6f14503b9ffbbc6c0304a54f4c88a8e3a9d141d6007cafd4990" },
                { "si", "384293a1c9d5d5ad09a1515c8215374cd63bdc0af5ce6e899227779ca25277385e44691f3cd50ad6eb8d05d5b888f1c9c3a2f180ba9027829f88a36d3efb94bf" },
                { "sk", "9a69d83f9b962efd9058a415a986a687541629b901f34363c7630740e27a0674de33297fbea80f2375906d2a0401ffd64ec6ce914493cdcebbb14f3936b7c93c" },
                { "sl", "1ea7f356e671a8abdf7c0d59c7dfd5882f50282f3da6f72dcdb374e48cded1106f888a3bdfdca494c9feacfc5f4b00fb1c3da686e9305921f42ed6ac82b503f2" },
                { "son", "b7b711878b7654e081d6ca492c74ab99a80d8c2146f1df6ba446971b352f5f8c0a46d810de315ff0a0477b33ade1525c4b1b93ab21a1134cea343f400ec07049" },
                { "sq", "55a6f70e671e1d61c971186dc598391cfc75e0980199ded5db76de9c5d1fe0c416e08badbe87b62edba00352e9afa11450f8c7e1cedef1a7b45be205f936228f" },
                { "sr", "70be300fa23b81db48e3c610cf3dae3d6237f02b0ab3a4f9cfef4086c734a9b9696a608240fdf2c547921453312dade39614ed0cecda190640b9fbc12d64893e" },
                { "sv-SE", "fd363f16091fb3b3a3a4cae0ea866e8732271d14b40a417121b50e208b2c5aeba2244658552cc2150df382a9db42e2ab29a2a14b0afb5feec0caf4be62fcea98" },
                { "szl", "ac927feea325afb3a2ed7e4e030cd6c1985971a718dbb6b6d27e68ecd21100a3b8e08cde9e421c4167ea653694a3df4bbab1654195d2417146add9914ce67931" },
                { "ta", "9550ac9518c4436d9a9edf5b90d81e6d7ccc0d235c5985f9aaf98e443ccc5dc1b602797a2905771f53afd6eb2a8af57d586b814f403ffb384f349339715e8967" },
                { "te", "72439ae8fa4346df1272730bca8de7bdec51cfd8056ee522fd3ad13f9c69aaa9b23109786567c068f2515a42417ddedbcad4e5c7a09e08299478e8842d57841b" },
                { "tg", "c8e7920919b73f209ed24f9783746ba50f7dcb63034732c8af399ad05ce5b8017b793ef9abc3687bb1de537c2386f41990c12c175f6fa9576bf157256e35d495" },
                { "th", "0560d2bd9ce726127c499902defcecc2a49e7ecaf459d75a0f5cbc374eebf429565b39dd5b617db57df3136518b9b22900e2032e5ec25f400ccfcb9becc0d809" },
                { "tl", "fba125490ab32d691fd36da9578be8f462b952230c81921b6f003d0edb63488b0a6639218b2ab843942a85b628728e03aa47d3e7b06d7419fcd9de7f0d0ac097" },
                { "tr", "2c8f390b4a64d60b02c53806c83805a7d03bf11dd3a1cbbc8de073182acc96156e6907ec5fee85a60b7aeec417c741acc7eec6dfcd1e2e1d30368be7d86f2282" },
                { "trs", "26f9680935cb94eed8142ed5eb5961f20ae0c5051825a27579c9d2709141dc8264a4b3460e4ab777ee191e6e7e41057d47a526bd69d34d7133f921d391ce8b60" },
                { "uk", "3c7ee7f7f0a013642a52e79469bfcfe1147acb566c4c1978fac6108f8113ff4ae6964d5edb621d437224d6e4af3bb34363ca4a0ff745db98f24f19ad5ccbd7be" },
                { "ur", "dacdb619576e40a7dabf37421290f5bca70c5357626a867b974588fca419599c2c8001fec176f3f96be3737c59160bd05184920b9ee18e8acc00051234b02703" },
                { "uz", "7de253018278eb70fe7c08bd73ee7995251d96b01c2e070e11e5d0ebad52450fe2d11b5b64c08fce3206f9f76838b28816ad78c13f9daacdcc9baec0e1f6102c" },
                { "vi", "c7cd81a3b3dc5fb37c7547a4109ffda04b6eff263a179615347c6fcb12bf0ecbf6733d6c23d9ad695975541ef5338bb2f53a631f3c79c2ec66ccb7947fca8eaf" },
                { "xh", "3ce29b39c01e5320b3c288c5d1258b2c70c30f0e2b2b9ffd8eaf2748175aa1108d89c0f7a7a2d0350831c805eb6ca76dafaa24d333693d37ce35d9071a3d3976" },
                { "zh-CN", "3613c32c3a7ab3d3d9afd300c9740b2a4129a8558347086db39d44afe248e5e466d202ac9e01b926e32b46d9fec9e196dd18b1f9a58ff1fc00d8f3b9f632b7a6" },
                { "zh-TW", "fcce19d0c480e2722c471fb981cc50100fb5c5130a0bb30b15c174cc989916083f2cb01b029e91cf9f4cd5fc9398fd7a68f95159953e41c08410980854e2f08d" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/115.0b9/SHA512SUMS
            return new Dictionary<string, string>(100)
            {
                { "ach", "0192a09b52ac3e2efe91a80a17d3d8cef2348a7b74bb9a23606a0d2c7a22e7682d828ac525315f043356a447eb715c3ac0259b8c13b64b46cb6198e5df136460" },
                { "af", "4f071015373357c1ba2e8c1c27469da958c63c2cc0586e2d7c757727d76d94127fa5df029e13247f8136c0e509385bb1fe172a9af9fe68b164b67c94b58e00b7" },
                { "an", "7d6f8522aab222dbb22b7e9f3435f3e6d7a49a0680bdb85b0403d89d310538be96c4198417c5ab045ddb5c677d2ed11ff5b473fd6871600d110de6427db3fc7e" },
                { "ar", "cce2ae24f796dd09369775ad3ba043313385943c9545205dbf6d926bc156f34379c24699c49fcf1dc76695ccc8f7f06c5811bf01b39628b3ebda494b9d241498" },
                { "ast", "f6b87ac055b878ad0636a799ad923d8edb7a4867cb706a454969d735c01d97c81b79568b8bf0ae09c3fee6eb3966e6eeb0299d762943cc3a704762ed2ef0da30" },
                { "az", "b7b577b678e6cd6e03b539f1dc305bc51b38ad969b62b219466a69b24b1c2f234e7538e7ad4d83be9fe3549f0f995b49f53906ee8dad57cedbd22bc204b0b9a0" },
                { "be", "92d0b9ad111013bb68399ba995642c0793761b5d942210f4cff914ca85b8b980137308f7961bc15bb1cf54f43875e367096082ed5b74078fda0b2f946d7b6349" },
                { "bg", "8c81ed3a2859a8bf4ac08d907820bd83848405ef37f775356a152eca6b89b793411a4a8935053228ea7de257796327eac4ebc81d064bffd6bfc75f008e6dcd61" },
                { "bn", "aeb1d082d032a31f357570366f86f3365ea24d244fb1f24ee1616e711160ad0cda2e70a93db8de005da6de5c875b157fed35b5a5c0fd9cbe5700ba33aa7b1d20" },
                { "br", "f732f20b686ff025219ed41dc2fd64793a63bf0836cbdc6d7e5e2d4856ba4b4d6b9beb77d3b5a731eb0cfaf653ba486a1e373a0c6fb1d192caa86f11553badc3" },
                { "bs", "aab9fbff820cd0bf81d1e56b533bf1a93c800e9f124a6ce0dd067a8c049c0bf9ddd3e9a5c762f248bbbe9cc33f15852fecac37fc915569002ffa69c5cf307c53" },
                { "ca", "ffeafd9a97c4d85e05913f1ae737328876aaf99b9ff87d03ac7cb4e0640f5a1256f16436d12c550912930321cbd83c3d853a6178b947c783ee7da90f22036f81" },
                { "cak", "65baa4bda8441e03bf24ee8656814632dff34828f03e3292fedb5c9cb05a4a6e06e6c232d58fa8c2b5b4c331ca12a536d99823625351e607f4dac72f8b53f8ae" },
                { "cs", "584c3ac5ea0a5dfef8fdcfecc84f5c1ab139598a4fb22eff730621f6073a60f192f02e7a32807f81105f14862e4a52a7edcf2cd94dfa71e109d17d7f72d5377e" },
                { "cy", "dd022f6070aa0501b9b9d6a791a7a7469b64cb156f27dfc0fd3f76fdddebe6c38bb8db71e4a0c5e89f22da45924334dc6f3633a56ea977d5f2efc965e9e42bde" },
                { "da", "251ba7595c21a79613df5fc34869280c7b4e2c7f598183d28e0e1b1f9101fa22ff0a76c384010d2cee3ed7876306a102a12a69ff8967964169bf5fb7c1977e66" },
                { "de", "436eb14f590271675eded56b18fb9281e4eeae2bfc9d199097867814f48d943f218e6317f3a418e3e27770f6d0e339a35d1de1026a534fecbf621fa20bd86bf0" },
                { "dsb", "55467a6d75a1c2cbb65539b1b4202ddbd3742faca8fe202f56b2c7023a01000928060888d355fae1dcedea7148753709257c4b255c9cae1d9948be6c2eb2acb9" },
                { "el", "bf6280562236392ed5827b8b81d6172993cc414a6c3317d33dc7c9b89b8a08ac00cac530658ffcb8fe345ffaa0041408b112e302b94f7768b3316bdc1f5b470d" },
                { "en-CA", "35adf541f016fe0466af7f90b0a39bea5aeb99ae4adda4077e9fd8ff9983558e4651033e54634969fe74bfab67719c7f2c259eff59cbcc20eb0bfcadfa2c0fc3" },
                { "en-GB", "9f6718d69767ae8d53fb5f2ea9f4e61516f0785a2a901f41b0bcebaed9d5fc223b754b540e68ac583e0c9fe8b21bb01324b95001f001d66e7709a83bed70a3ee" },
                { "en-US", "0ee6e3864c376cc59483effaa66f6d5c27f4a9a5eb29763dc6751c9e2c706dba9750ce6634f4e646321f92241fcb828b107feb0c5167b553872096ea803e65dd" },
                { "eo", "abb7852a1f2eaa4bc322c3a7287028cb28d23a71688527284ce108bc10670eaecc19819d9dd5a7d125bef5192ed8018adb0a97e55cfab6cacfe1c36244ab49a5" },
                { "es-AR", "2a7809258f5d5e1ce86742ac05f0d7f2b03be0efb61883d0e1037b4f10b44440f08fbc0e4875c38154740c3dda36b1b8a8ec68e2023fd96a503a62dd25b0438e" },
                { "es-CL", "972b5a983df1ba23e69065ba36594e68d159e376872f39ad61fc5c0a6cd50c832c08d8356af65fcdea3ae7aae7bcc08b439d23269500751459c4c95889f7759e" },
                { "es-ES", "8033c0df81ed4ded06050905351925dd72aced288c8fbe0e4420ddbdb921c1f7faf2be25aa5458b6a8372cff930dc3164a49a534d0c73852a08521b421b755f7" },
                { "es-MX", "2e0b2dd140e1a25a181dd5edc3078cd89975700ed3ea43770fcf3cd849f6d873e912cc72ebb49080bc5b7e3a4de601ca2fa3befd72b68b0e82d55a338cd51c7d" },
                { "et", "132b425840f787eaadea5625bb874156f0453f78b55d83711fd7488dbcc7dbca47cf3c7d840e5cd8a0e4c921d9ccba43de7046fc4daaff60662b76100baa21b2" },
                { "eu", "ff1efc7a28d8862cd5030353aee6ce2b10ad802bb319ae678d11439cb0152a51e9080e8d9bb8eb141667872fe6b2f863ab593e1e3155c6e2fafaf4aec3fa2a91" },
                { "fa", "a364f716aea3176e33e91c3c605c5d813511d07bc416eae5aa66a8a32a90d72b6ea7fa6c71d1d22cdebdb57908d395003985a8657ef8ef30c6a75694946e712c" },
                { "ff", "b924cc812aebb0c0488f5e4b6f5a5f814d8a925264ba695a619cede9c38cc33fb8b20d8319a6b5ccf7840c6740c32e670dd48c0d93d5e8369634c865092b40a4" },
                { "fi", "f6ec540464151325abac491b24f45cde4d089ac78542c6b6d4d8a410ca1e771fe089260ed03492a5f15d2cf36f7cd152695a6aad2c9b53d166d6616ed849c690" },
                { "fr", "413f76119fbbfd8818e5460ff815baba4ac438e56d1a0ef5bcb3adc2415e2ea618de4263a215b471b6dba1c3baf23ec1da3affbaea4a1fb9174c37650be58665" },
                { "fur", "17f7e66f459dd5624bc0036326bf5fb6ab3ca6f9b9d1ee0c84a0406223f1c955aa0106d2c2dd27c9b472a5819e37f0508f31750891712135909e36667663e25e" },
                { "fy-NL", "4490261d4430ae1bc9c4f2ec7f1303a4ab2c2d7b60a6389231a863485c9a3d7405f811c3782ad65534152734c3d875dd6175ca7a50ef94551c22abe197a5b855" },
                { "ga-IE", "a64064bdb775905734238aa191afc37d3a348936e4b5ee958c770e3d29facdb62c65be54dc7fa63b0176e3ac27b9f3a25f4ad465c370f11928bcd26805316291" },
                { "gd", "b262998d45a70a70ec06c5980107ce4454ce0c84eac570d55ef37e15b2216b8cb530301bcdcad43eb9be582edad11902b0f5f6999fb1bfe7af213854367cb743" },
                { "gl", "03c24e4e2230ebdd55cde3a6fb95052c35b29a725d00df456c7dacf9879110cf4bac8ec094d0a352a9a1d540014f7e1ac84efa36bf41e4858ac8006b0d906383" },
                { "gn", "2544a074a884d053848eb9047cb89bcfa62f86dd64693adae9ce168e13f5df3ea7c94674e40f6877e508f259ef0f9c0251772c3a9ffe604a95b4af64e155efd5" },
                { "gu-IN", "808ce1f12046a2b192533e998567aba2c588da6b8faf5cbc1f3e87f26c895b85c77dcf1dcf6c7771e603157728d84504bfbe83ec3fd6e897f29ea1338328580c" },
                { "he", "765df33ad7d0e1dd7c5f4473814ded70788d7f276ae830a40579678be706e44b1e2ebc6baa7afa15483fd203e0917b9ede4c347e57ee39af217630ffe7a30036" },
                { "hi-IN", "1999e0abda6acbe38c8c43871c9a5169a91824965677b856651caf3c91a5d814a5fa651679653f04560d20d3502393832d39639472372a80a65eb492d4657c7d" },
                { "hr", "7191fa4a64fa0848af4f72135173d1dee950bdadb4d7b3eda594c91e93591a4bebac9872e78df151a01302462590f757e901e6c493f15e21bcf27b483191a46c" },
                { "hsb", "e5c41f6b2b9fb27b67d65d02378aaaf43067114d3b43215e862a5ea6cc1982622c5bc3bf0663e061735378ccc8f1ce1a63989bdd5c3171e450f1cecb100f1f6c" },
                { "hu", "bfe2f0873948e929e0f17c9f1b14331c77dd8a0b890b68209bf189a16d923abdb699f20df96ca439ad6e314afaee86a5f83e0f59de471ab580b0893b7997e44e" },
                { "hy-AM", "8c8b583ef38a78a782c9c6d36e88d1481dae9f9192ed1fc1595a5bf5f99e8c1895ee873bebd980e6dd64f58510e73e911189e864915fbc1ee524431e55e08bf2" },
                { "ia", "b55229518f4bd00962e7bfde7f981ba2206a069214e81e5b9f04c0ff97cc1e176ff1dc151dbd1a36b35e5d74548b801137b253bf585112ef7158a9a18cb226ff" },
                { "id", "1c3ec74aa92a9b218c651eeed23ed5775933c253328571195e6b0c55a6aea494a73044843d377eb4115ac1d80105b0e5cf253930247b3e5e10bdb7e09635e558" },
                { "is", "3cce1b37ee4e1bda6b777e268f0d00582690c3986e94f521f96d0a9c60e75a370eba55082b6ff1a4b5c6eb92124fc461b66de5d15213bc30a265b8a236450d32" },
                { "it", "13e0f97726ab1cced632967742ec797aa53b6af2150e33b9570cd75f3db9dc753f2548231dd8a462be04ed740f5058bba9fc8ceda7376120cf3a5ad66b5c058b" },
                { "ja", "e3720e6e9848078ab8505637ffd0946af79c093d197044f5003fc96c95fe77eac1cf538beb4256152600c0ad25d3f38c8c90ad0290c8f60274017abd66088882" },
                { "ka", "6ec872783cc03a6825e5976508aed5a805350bd9314828acc87b6c7599961249eb57af785f1d0c346df7c7c08d8246d9c8a445100241107e8bbfb3a2ff1174bf" },
                { "kab", "f2855cd3cb72040a522a28299e4b326488c3ffb80873d3e846aefd9fbbb8a34a7d742608a7b22c62e4d65db2f77ed81e63f730d92e9b0e2a1d7634d2039d1d9c" },
                { "kk", "6378f9cc005b18934a665fe8d69716f2b900ecbc16c23a92ef81d13e33b31f0be201512057fa1a96e109ca34e5f0a48a5084eec70fe20593db3a811754cdf281" },
                { "km", "436fa755e01746d80204f2c562bf247d09546ef242fb510ce7a892a792dfd20ff09e4e7ac7d9779db35b5a63657f5dc2606ccb9d54b749a4cb1943ba4289fa8d" },
                { "kn", "60c1fca7373e11e260811d5d7a708b26ca4d6989cbe0627595d7c9b65de67ab6a053a932f5385cfa592eed56dccfdf6bcb0793dfdae0a29bc7225e1e263741f7" },
                { "ko", "e0211f734c1868d827be42806722fd4aa9c90e07eb6c64f8430264338058fe0b829d92c4cef7f92aac90680a533a843d73421a717b279fdbd0a0fc27865d84e8" },
                { "lij", "08a3ddd0a5a82d0c67b705484894f8a42bd66cc154405b434640a9f690d4242124661d7ff3bab3cfaa4cad79af1b66e3df68b5f053271d10e7d3c22e6120d57f" },
                { "lt", "289b5681724d1303c9843857b15e88dce0141a2735b833247ae17abf14a51cc4308cb9b20d2fd02b21d5597754fb867b2ec974c480d21e3ab32873e485bfbe4e" },
                { "lv", "adb8cadf50303ee7f2c9d0d9115994713d356f8c0dffa800c7582abd649b25fa126ed253964ef0a00efcfc197979a5c4596654f4b21a7fbe72fa92174dad0ef4" },
                { "mk", "505b57f6121a19ce2ec12baaccedbdd61e99912013e15e167ece832f205682b115f5ce1358db7764f166e7089e461c3c84e9ff6cf3f5613751006f1622347c0c" },
                { "mr", "845abc66cf19b7c62c8e1e561390fb6ee32d8ed3bd31f23ecfce505010f73a6421e6e915953cc990e009b9cb5993a54815fa66d948cc012b40426322940fd9ad" },
                { "ms", "4ae17b4b43e921368a6a0a8522f9f48f3d5279eff564dc2672d815e452fcb1d6309e97f92bf5ba4b88e30d81ad15062ae6277c466fbfe567810c484ad2266c04" },
                { "my", "a16808393fb5c1b1cbfc20e47e3a50bcecd39c6e2dd74c14de6d96c6fe57ea19d0866247469a5e6ea744d9c45bfa3c2e563edf624010e6eabb89ef796f509755" },
                { "nb-NO", "8fb0a51381015ac5970d65f58a416e5a086031cab8119c7b4e1cad998fbcd974bbb33513791f3354be112d8da2a49eeee17a121930f4669c0d752d42c48cab36" },
                { "ne-NP", "37267e231fc5aa788f4a0667b340ca7167b4d544f3ecac28f98fdd02b917057a3bd4fa65300436f99585d739abed70277ba35c132beb639b47cf08a32105125b" },
                { "nl", "626a8bf60398e1653260ed41c8df0ac98b2ba8bda082ef6011dd3d086703cbe9a22b1315c2fcb3775d9809e3d6c45af8d46f91f46899b664faa6448bbf31d03c" },
                { "nn-NO", "7eacc1fc9a6b439cf29cf1f5109d5f79a6c9fdb41a1c295ea261f4152f381c3449827eb39801468bf47b802297217d851c6abe15fd8aebf71b8afe219d2715cc" },
                { "oc", "6fa8e147292c3e66ab16f1852e54ad7fc98547c4d4db11d39c7c95c99e65d8e05a9e2a45b2561ee5e19992e740dcb2e93126dd032b22cdea19edfc8aab1e49bc" },
                { "pa-IN", "f6b25bac5781a5cfe12f358975f09e0162b77dbdd46a4d8ab9bddd0bf062e6d6d8e7c28192eaf57a82ea6717333acf427a3943c6d24079d5d8d572f2aca62cd8" },
                { "pl", "aa61f572a95d2c28138f0a2f775fbfa030678ceee9f2bf12f8d936d54cca1ce511b602f6af23dd0f2acea1551ce018bb71018d3430ffc43061d6610d89db7cd0" },
                { "pt-BR", "1cd311ec6b82ad5ce24598388cfefb845cb1500007c4c6d92e7e355e11ecd6fcc5336db4fff324357b2740f2df62dadb3d393dd67880b4757b8f70731c278a9c" },
                { "pt-PT", "87ec9c2210ffb495af9a9af8fe09958db8cf2f5b1d8dad2745dc5402c906bfbfca44f30a6363ca4bcbb30c8054ce4969103c324f8a77bb21f9bb76e39ece9d5a" },
                { "rm", "c2c0a84bc5cca2e5f3a3af78fae4e4470565fb09034be7b01b558b0543e3d5315b636f5d5d763ba9e684a1305047c4a9c4408bb67d2a67e86a12ef0d9105f212" },
                { "ro", "efcdcca05dac599eee289061484f8dd7a1e60f68ce3e3265c2ec48d24f48bdcb9a3046061af3eb4b09290bdd075d1bf47516ecd88fcd3b53e557d848b23ec2ab" },
                { "ru", "5bc505bdd9f3e1a773426112771195c9cc1068c8e25258ba449324da4594f7cdc1539a1f3ae7bf4cbbc5df2ef09da282f9cc0101bcf4d90faae2b9618670a045" },
                { "sc", "47e4a293d4f688e6aa8ba43c9f8453d11718d173bb60eb751859fb5af6e4a8f4e8a9fc461ebb2bb6e263e5a00d27103feff66ddfea66857ce1756a56e236aa54" },
                { "sco", "86906258553c8084e1b9b98d08cb8b6b5e9f31da01f8fc44e7ed206411dc9c24a561429d69cd6c8fe52b504a3c243b0ebcbbf7a8c1e75b63c355aa93197baaf9" },
                { "si", "b365ef67d1e1f3cfcf01eb410efa407d2b1ac3f7efc6f2f5c1fb2ff9cb4d4fdc069b78b88967a38ddc470164e293fd2c1b0fa64b545701f609872e897ac8c267" },
                { "sk", "70bacac16ae43196b9af9ee1802c290a311659fcba4ad086c8ddd9e6aae569ed82d91b8890a48bb6d809fed10553b4bc947bbfab29864bc0386dba65c37c9340" },
                { "sl", "337f905308fc04169cb533d127213788614888dd9d1158b188aa667aec5b731ea98e13e4300517dbeb49b5830a3249fafc2ef1a3f5b1531ba849df64827d8732" },
                { "son", "ef6d05ee1f63f1231f50837f1deb422deb35fac7a8a87843d0724401309c06f8ff9fd37f6607741fdc09e32dd997f695f1c325ec14fe84466250ccfec51dd266" },
                { "sq", "dee6523c4ce1d646a61e51a6425da0e2c236d366fd3a0397bf44d21b90750d9c67aef5ff7af0516997b34b067eddd65cfe918e6058c80d9e3dc39ce8b0a29ffc" },
                { "sr", "4bd65532f33da8f8b04baeed0d2e858cecb356ae9bb3d56f86719efbcfd7ffefda27c57a893b684807294dc4246d28391e567266086e4d7a560a878a5850ac3b" },
                { "sv-SE", "c9492d0a7be0eaf2badcb2e643bbd50fbb5191a1634440792653069ecf5423f98af43fc12babd74289a695243f720b89564dd4ef3a29ab6cefa93edb3beb0e8a" },
                { "szl", "909470c1a6f0a328fd795a18f00da786f5142c02373d5779f2cef5267d0ae290639ca6f0491b6f783b0575e280c3597c9811a43e7fc9100227356009c4a80630" },
                { "ta", "209363fe9dbefb0b26b9a15cc03cd376eaffd63865ebcfe3e256ee039cf0681d81c2ef7aa94050e1158c4185ea4c63c0c72d4fa13a406a4b97c1787cb4a3c632" },
                { "te", "7fc50fd2ad9aa6f8301caa7ef87229fc81dd1d1f69105f1d9b35e1e8b76881098e0ee232c5042595572a84dfd8af63c9c66ba20e6bf2493c929248a79e56e296" },
                { "tg", "2e982809d7d005f1e85185579b3d0a25ca17bcf3a3a1ca5db827284410e9eafb0b54ef50522e7afad531a3fc232a1f13499e529fd80b019488c88bcd816266fb" },
                { "th", "1f81c5e9ba82578a7c418516c1a8ed9f25f4d330b43a67a97cadeb93c0c2777c4fd93d33e0a9814de282920fe37027e587cea7df1607d631222e8aba8abc6603" },
                { "tl", "d7037dd369d3babb3d9a88403a894a8d731648fc12a1c3c29ce1e9a8a28a9830d9f7dbc2cfc847da11859d86a3b45fb2f11302ea8b3f5093de8f8d0f81b8f287" },
                { "tr", "0318c8b161f99ebf594c6f2ab871ec13197948b5b195ec5438fe06bf791072fd51fd24d4c67da04531daa2f0a7958f60fd09929a9acf289485d8e6a1e30b8516" },
                { "trs", "5aa2ee4b3e4e7b8b94c765227511dddc898fac90dea3c1176c0a410ec40e0f591c185c6f5cd272afef0c307016360a09cd3339f02e13a267856139264eb70027" },
                { "uk", "485fbc54702ad310c02caa1e0602e0bc72d9bb1a52ed981358559ecc00cc3a9550b461818c603ba3a2fd5e71d0adeeb168cf8c05fa37d93f11b3e5b2064101db" },
                { "ur", "b614f7dcf06ff7e9e17117a9f8cff631c5bbd0ef19c17be3be61f7b611109157e193a77b03795d4b5f06b82fda049f5c7c21e3a6ecbf56f9d1519f7a3cc6db9b" },
                { "uz", "a897ec6ee862b61ab0dc7431a71331e0d8db0d5f8fd5dbdbc119fa9e36561b69eea83195aec0cabd179de0d49cf46bcbacc590bf9360dfab5f3591b13f843fc7" },
                { "vi", "dde1ba033a8516ee34897bf7428cbade9c2e892ea94e55394160f217986f1a81a733600ab7d80ba331f0f1d3a2ad58867530038918581dfcb203d12c62930a31" },
                { "xh", "f7ff9e48389b77c97bfb620b3632f38729b1a566acedd24b28c84241e1ae79bf53aa67eeafae28526b52de8ed53135dca2445ff543f30c44461df9993eaf8800" },
                { "zh-CN", "6897ade513946df8ffeaeb9640536791b07831e021d4d5aa96b61a5bccc8a2e7503d98e5a178c13e636be8572f16a3dd989682d6d15cb9cab6a35e3690d20db1" },
                { "zh-TW", "37b103cd2b8ae2107697f285b15b9d5c3631b074cfc5455673340d3ef89b1c1ec0c676e37e60259e6cc483bbe0e79a5b693654164850a9f3c861ad9b54ed3464" }
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
